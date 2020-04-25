using System.Collections.Generic;
using System.Linq;
using System;

namespace sudoku_solver
{
	public partial class Solver
	{
		private readonly SudokuField _field;

		private IDictionary<int, IList<int>> _candidates;

		public Solver(SudokuField field)
		{
			_field = field;
		}

		public void Solve()
		{
			InitCandidates();
			Console.WriteLine($"Candidates: {_candidates.Sum(item => item.Value.Count())}");
			Console.WriteLine($"Candidates for pos 0: {string.Join(",", _candidates[0])}");
			FindCandidates();
			Console.WriteLine($"Candidates: {_candidates.Sum(item => item.Value.Count())}");
			Console.WriteLine($"Candidates for pos 0: {string.Join(",", _candidates[0])}");
			_field.DrawField();
		}

		private void InitCandidates()
		{
			_candidates = new Dictionary<int, IList<int>>();
			for (int i = 0; i < 81; i++)
			{
				if (_field.Field[i] == 0)
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
			for (int index = 0; index < 100; index++)
			{
				for (int i = 0; i < 81; i++)
				{
					var pos = new Position(i);
					if (_field.Field[i] == 0)
					{
						ClearCandidates(pos.Pos, pos.BoxIndex);
						ClearCandidates(pos.Pos, pos.RowIndex);
						ClearCandidates(pos.Pos, pos.ColumnIndex);
					}
				}
			}
		}

		private void ClearCandidates(int pos, IEnumerable<int> indexList)
		{
			foreach (var index in indexList)
			{
				if (index != pos)
				{
					if (_field.Field[index] != 0)
					{
						_candidates[pos].Remove(_field.Field[index]);
						if (_candidates[pos].Count == 1)
						{
							_field.Field[pos] = _candidates[pos].First();
							_candidates[pos].Clear();
						}
					}
				}
			}
		}
	}
}