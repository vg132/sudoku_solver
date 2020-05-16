using System.Collections.Generic;

namespace sudoku_solver
{
	public class NetCommand
	{
		public string Command { get; set; }
		public IList<int> Field { get; set; }
	}
}