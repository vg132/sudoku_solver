using System.Collections.Generic;
using System.Linq;
using System;

namespace sudoku_solver
{
	public class Position
	{
		private int _pos;
		public Position(int pos)
		{
			_pos = pos;
			Row = _pos / 9;
			Column = pos - (Row * 9);
			BlockRow = (Row > 0 ? Row / 3 : 0) * 3;
			BlockColumn = (Column / 3) * 3;
		}

		public override string ToString()
		{
			Console.WriteLine($"Pos: {Pos}, Row: {Row}, Column: {Column}, Block Row: {BlockRow}, Block Colum: {BlockColumn}");
			Console.WriteLine($"Box index: {string.Join(",", BoxIndex)}");
			Console.WriteLine($"Column index: {string.Join(",", ColumnIndex)}");
			Console.WriteLine($"Row index: {string.Join(",", RowIndex)}");
			Console.WriteLine($"Box column index: {string.Join(",", BoxColumnIndex)}");
			Console.WriteLine($"Box row index: {string.Join(",", BoxRowIndex)}");
			Console.WriteLine();
			return string.Empty;
		}

		public int Pos => _pos;
		public int Row { get; private set; }
		public int Column { get; private set; }
		public int BlockRow { get; private set; }
		public int BlockColumn { get; private set; }

		public IEnumerable<int> BoxIndex
		{
			get
			{
				var startIndex = (BlockRow * 9) + BlockColumn;
				for (int i = 0; i < 3; i++)
				{
					for (int innerIndex = 0; innerIndex < 3; innerIndex++)
					{
						yield return startIndex + ((i * 9) + innerIndex);
					}
				}
			}
		}

		public IEnumerable<int> BoxRowIndex
		{
			get
			{
				return Enumerable.Range(Row * 9, 3);
			}
		}

		public IEnumerable<int> BoxColumnIndex
		{
			get
			{
				for (int i = 0; i < 3; i++)
				{
					yield return Column + (i * 9);
				}
			}
		}

		public IEnumerable<int> RowIndex
		{
			get
			{
				return Enumerable.Range(Row * 9, 9);
			}
		}

		public IEnumerable<int> ColumnIndex
		{
			get
			{
				for (int i = 0; i < 9; i++)
				{
					yield return Column + (i * 9);
				}
			}
		}
	}
}