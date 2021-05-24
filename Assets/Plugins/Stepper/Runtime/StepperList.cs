using System.Collections.Generic;
using Stepper;

public class StepperList<T> : IStep where T : IStep
{
	private readonly List<T>  _current;
	private readonly Queue<T> _addQueue;
	private readonly Queue<T> _removeQueue;

	public IReadOnlyList<T> Current => _current;

	public StepperList(IEnumerable<T> steps = null)
	{
		var stepsArray = steps ?? new T[0];
		_current     = new List<T>(stepsArray);
		_addQueue    = new Queue<T>();
		_removeQueue = new Queue<T>();
	}

	public void Add(T step) => _addQueue.Enqueue(step);

	public void Remove(T step) => _removeQueue.Enqueue(step);

	public void Step(in StepData stepData)
	{
		while (_removeQueue.Count > 0)
			_current.Remove(_removeQueue.Dequeue());

		while (_addQueue.Count > 0)
			_current.Add(_addQueue.Dequeue());

		foreach (var step in _current)
			step.Step(in stepData);
	}

	public bool Contains(T step) => _addQueue.Contains(step) || _current.Contains(step);
}