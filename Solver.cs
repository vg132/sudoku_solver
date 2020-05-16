using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace sudoku_solver
{
	public partial class Solver
	{
		private int[] _field;
		private IDictionary<int, IList<int>> _candidates;

		public void Solve(string field)
		{
			_field = new int[81];
			for (int i = 0; i < field.Length; i++)
			{
				_field[i] = int.Parse(field.Substring(i, 1));
			}

			InitCandidates();
			Console.WriteLine($"Unsolved: {_field.Where(item => item == 0).Count()}, Total candidates: {_candidates.Sum(item => item.Value.Count())}");
			FindCandidates();
			Console.WriteLine($"Is correct: {CheckIfCorrect()}");
			Console.WriteLine($"Unsolved: {_field.Where(item => item == 0).Count()}, Total candidates: {_candidates.Sum(item => item.Value.Count())}");
			DrawField();
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
			for (int index = 0; index < 200; index++)
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

						FindNakedPair(pos.Pos, pos.BoxIndex);
						FindNakedPair(pos.Pos, pos.RowIndex);
						FindNakedPair(pos.Pos, pos.ColumnIndex);

						FindNakedTriple(pos.Pos, pos.BoxIndex);
						FindNakedTriple(pos.Pos, pos.RowIndex);
						FindNakedTriple(pos.Pos, pos.ColumnIndex);

						//var text = File.ReadAllText("visulizer.html");

						//text = text.Replace("{{NUMBERS}}", "[" + string.Join(",", _field) + "]");

						//var candidates = _candidates.Keys.OrderBy(item => item).Select(item => "[" + string.Join(",", _candidates[item]) + "]");
						//text = text.Replace("{{CANDIDATES}}", "[" + string.Join(",", candidates) + "]");
						//File.WriteAllText("visulizer_working.html", text);
						///Console.WriteLine($"Index: {index}, Pos: {pos}");
						//Console.ReadLine();
					}
				}
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
					break;
				}
				if (_candidates[index].Count() < 2 || _candidates[index].Count() > 3)
				{
					break;
				}
				if (_candidates[index].Except(_candidates[pos]).Count() > 1)
				{
					break;
				}
				nakedTripleIndex.Add(index);
			}
			if (nakedTripleIndex.Count() == 3)
			{
				var distinctList = _candidates[nakedTripleIndex[0]].Union(_candidates[nakedTripleIndex[1]]).Union(_candidates[nakedTripleIndex[2]]);
				if (distinctList.Count() > 3)
				{
					return;
				}
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
					break;
				}
				if (_candidates[index].Count != 2)
				{
					break;
				}
				if (_candidates[pos].Except(_candidates[index]).Any())
				{
					break;
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
						if (_candidates[pos].Count == 1)
						{
							_field[pos] = _candidates[pos].First();
							_candidates[pos].Clear();
						}
					}
				}
			}
		}

		public void DrawField()
		{
			Console.WriteLine();
			Console.WriteLine("===================");
			Console.WriteLine("====== Field ======");
			Console.WriteLine();
			for (int line = 0; line < 9; line++)
			{
				Console.Write("\n|");
				DrawLine(line);
			}
		}

		private void DrawLine(int line)
		{
			for (int i = 0; i < 9; i++)
			{
				Console.Write("{0}|", _field[(line * 9) + i]);
			}
		}
	}
}