using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace sudoku_solver
{
	public class Solver
	{
		public IList<string> _steps = new List<string>();
		public int[] _field;
		public IDictionary<int, IList<int>> _candidates;

		public void Setup(IList<int> field)
		{
			_field = field.ToArray();
			_steps = new List<string>();
			InitCandidates();
		}

		public void Solve()
		{
			for (int i = 0; i < 200; i++)
			{
				FindCandidates();
			}
		}

		public void Step()
		{
			FindCandidates();
		}

		private bool CheckIfCorrect()
		{
			for (int i = 0; i < 81; i++)
			{
				var pos = new Position(i);
				if (!IsCorrect(pos.RowIndex))
				{
					return false;
				}
				if (!IsCorrect(pos.BoxIndex))
				{
					return false;
				}
				if (!IsCorrect(pos.ColumnIndex))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsCorrect(IEnumerable<int> indexList)
		{
			return _field.Where((item, index) => indexList.Contains(index)).Sum() == 45;
		}

		private void InitCandidates()
		{
			_candidates = new Dictionary<int, IList<int>>();
			for (int i = 0; i < 81; i++)
			{
				if (_field[i] == 0)
				{
					_candidates.Add(i, Enumerable.Range(1, 9).ToList());
				}
				else
				{
					_candidates.Add(i, new List<int>());
				}
			}
		}

		private void FindCandidates()
		{
			for (int i = 0; i < 81; i++)
			{
				var pos = new Position(i);
				if (_field[i] == 0)
				{
					ClearCandidates(pos.Pos, pos.BoxIndex);
					ClearCandidates(pos.Pos, pos.RowIndex);
					ClearCandidates(pos.Pos, pos.ColumnIndex);

					FindUniqueCandidates(pos.Pos, pos.BoxIndex);
					FindUniqueCandidates(pos.Pos, pos.RowIndex);
					FindUniqueCandidates(pos.Pos, pos.ColumnIndex);

					//FindBoxUniquePair(pos);

					FindNakedPair(pos.Pos, pos.BoxIndex);
					FindNakedPair(pos.Pos, pos.RowIndex);
					FindNakedPair(pos.Pos, pos.ColumnIndex);

					FindNakedTriple(pos.Pos, pos.BoxIndex);
					FindNakedTriple(pos.Pos, pos.RowIndex);
					FindNakedTriple(pos.Pos, pos.ColumnIndex);
				}
			}
		}

		private void FindBoxUniquePair(Position pos)
		{
			var candidates = new List<int>();
			foreach (var index in pos.BoxRowIndex)
			{
				candidates.AddRange(_candidates[index]);
			}
			candidates = candidates.Distinct().ToList();
			if (!candidates.Any())
			{
				return;
			}

			foreach (var index in pos.BoxIndex)
			{
				if (pos.BoxRowIndex.Contains(index))
				{
					continue;
				}
				candidates = candidates.Except(_candidates[index]).ToList();
			}
			if (!candidates.Any())
			{
				return;
			}
			foreach (var index in pos.RowIndex)
			{
				if (pos.BoxRowIndex.Contains(index))
				{
					continue;
				}
				candidates.ForEach(item => _candidates[index].Remove(item));
			}
		}

		private void FindNakedTriple(int pos, IEnumerable<int> indexList)
		{
			if (_candidates[pos].Count != 3)
			{
				return;
			}
			var nakedTripleIndex = new List<int>();
			nakedTripleIndex.Add(pos);
			foreach (var index in indexList)
			{
				if (index == pos)
				{
					continue;
				}
				if (_candidates[index].Count() != 2 && _candidates[index].Count() != 3)
				{
					continue;
				}
				if (!_candidates[index].All(item => _candidates[pos].Contains(item)))
				{
					continue;
				}
				nakedTripleIndex.Add(index);
			}
			if (nakedTripleIndex.Count() == 3)
			{
				var distinctList = nakedTripleIndex.SelectMany(item => _candidates[item]).Distinct();
				foreach (var index in indexList)
				{
					if (!nakedTripleIndex.Contains(index))
					{
						foreach (var candidateToRemove in distinctList)
						{
							_candidates[index].Remove(candidateToRemove);
						}
					}
				}
			}
		}

		private void FindNakedPair(int pos, IEnumerable<int> indexList)
		{
			if (_candidates[pos].Count != 2)
			{
				return;
			}
			int nakedPairIndex = -1;
			foreach (var index in indexList)
			{
				if (index == pos)
				{
					continue;
				}
				if (_candidates[index].Count != 2)
				{
					continue;
				}
				if (_candidates[pos].Except(_candidates[index]).Any())
				{
					continue;
				}
				nakedPairIndex = index;
			}
			if (nakedPairIndex > 0)
			{
				foreach (var index in indexList)
				{
					if (index != nakedPairIndex && index != pos)
					{
						foreach (var candidateToRemove in _candidates[pos])
						{
							_candidates[index].Remove(candidateToRemove);
						}
					}
				}
			}
		}

		private void FindUniqueCandidates(int pos, IEnumerable<int> indexList)
		{
			for (int i = 0; i < _candidates[pos].Count(); i++)
			{
				var candidate = _candidates[pos][i];
				var unique = true;
				foreach (var index in indexList)
				{
					if (index != pos && _candidates[index].Contains(candidate))
					{
						unique = false;
						break;
					}
				}
				if (unique)
				{
					_steps.Add($"Unique candidate found for pos: {pos}, {candidate}");
					_field[pos] = candidate;
					_candidates[pos].Clear();
				}
			}
		}

		private void ClearCandidates(int pos, IEnumerable<int> indexList)
		{
			foreach (var index in indexList)
			{
				if (index != pos)
				{
					if (_field[index] != 0)
					{
						_candidates[pos].Remove(_field[index]);
						_steps.Add($"Removed candidate: {_field[index]} from pos {pos} because it's already unique in pos: {index}");
						if (_candidates[pos].Count == 1)
						{
							_steps.Add($"Only one candidate for pos: {pos}, number must be {_candidates[pos].First()}");
							_field[pos] = _candidates[pos].First();
							_candidates[pos].Clear();
						}
					}
				}
			}
		}
	}
}