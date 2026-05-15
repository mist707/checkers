using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp2
{
	public enum Player { Yellow, Red }

	public class Piece
	{
		public Player Player { get; set; }
		public bool IsKing { get; set; }
	}
}
