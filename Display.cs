using System;

namespace sudoku_solver
{
	public class SudokuField
	{
		private int[] _field;

		public int[] Field => _field;

		public SudokuField(int[] field)
		{
			_field = field;
		}

		public SudokuField(string field)
		{
			_field = new int[81];
			for (int i = 0; i < field.Length; i++)
			{
				_field[i] = int.Parse(field.Substring(i, 1));
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
				Console.Write("{0}|", Field[(line * 9) + i]);
			}
		}
	}
}