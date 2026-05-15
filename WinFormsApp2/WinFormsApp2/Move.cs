using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp2
{
	public class Move
	{
		public int FromRow { get; set; }
		public int FromCol { get; set; }
		public int ToRow { get; set; }
		public int ToCol { get; set; }
		public bool IsCapture { get; set; }
		public List<(int, int)> CapturedPieces { get; set; }

		public Move(int fromRow, int fromCol, int toRow, int toCol, bool isCapture, List<(int, int)> captured)
		{
			FromRow = fromRow;
			FromCol = fromCol;
			ToRow = toRow;
			ToCol = toCol;
			IsCapture = isCapture;
			CapturedPieces = captured;
		}
	}
}
