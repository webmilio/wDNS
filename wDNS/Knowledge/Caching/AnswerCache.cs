﻿using wDNS.Common.Extensions;
using wDNS.Common.Models;

namespace wDNS.Knowledge.Caching;

public class AnswerCache : IAnswerCache
{
    private record ToDecache(Question Question, Answer Answer);

    private readonly ILogger<AnswerCache> _logger;

    private readonly Dictionary<Question, List<Answer>> _questions = [];
    private readonly List<ToDecache> _decache = [];

    private volatile object _lock = new();

    public AnswerCache(ILogger<AnswerCache> logger)
    {
        _logger = logger;
    }

    public bool TryAnswer(Question question, out IList<Answer> answers)
    {
        answers = null;

        lock (_lock)
        {
            if (!_questions.TryGetValue(question, out var mAnswers))
            {
                return false;
            }

            var xAnswers = new Answer[mAnswers.Count];
            mAnswers.CopyTo(xAnswers);

            answers = xAnswers;
        }

        return true;
    }

    public void Update()
    {
        foreach (var kvp in _questions)
        {
            foreach (var answer in kvp.Value)
            {
                answer.TTL--;

                if (answer.TTL <= 0)
                {
                    _decache.Add(new(kvp.Key, answer));
                }
            }
        }

        lock (_lock)
        {
            for (int i = 0; i < _decache.Count; i++)
            {
                Remove(_decache[i].Question, _decache[i].Answer);
            }

            _decache.Clear();
        }
    }

    public void Add(Question question, IList<Answer> answers)
    {
        lock (_lock)
        {
            var mAnswers = _questions.GetOrProvide(question, () => new());
            mAnswers.AddRange(answers);
        }
    }

    public void Remove(Question question, Answer answer)
    {
        lock (_lock)
        {
            _questions[question].Remove(answer);

            if (_questions[question].Count <= 0)
            {
                _questions.Remove(question);
            }
        }
    }
}