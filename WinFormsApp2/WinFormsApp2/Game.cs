using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{

	public class Game
	{
		public Piece[,] Board { get; private set; }
		public Player CurrentPlayer { get; private set; }

		public Game()
		{
			Board = new Piece[8, 8];
			InitializeBoard();
			CurrentPlayer = Player.Yellow;
		}

		private void InitializeBoard()
		{
			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					if ((row + col) % 2 == 1)
					{
						if (row < 3)
						{
							Board[row, col] = new Piece { Player = Player.Yellow, IsKing = false };
						}
						else if (row > 4) 
						{ 
							Board[row, col] = new Piece { Player = Player.Red, IsKing = false }; 
						}
						else
						{
							Board[row, col] = null;
						}	
					}
					else
					{
						Board[row, col] = null;
					}
						
				}
			}
		}

		public List<Move> GetAllowedMoves()
		{
			var captureMoves = new List<Move>();
			var simpleMoves = new List<Move>();

			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					var piece = Board[row, col];
					if (piece != null && piece.Player == CurrentPlayer)
					{
						captureMoves.AddRange(GetCaptureMovesForPiece(row, col));
						if (captureMoves.Count == 0)
						{
							simpleMoves.AddRange(GetSimpleMovesForPiece(row, col));
						}
							
					}
				}
			}
			return captureMoves.Count > 0 ? captureMoves : simpleMoves;
		}

		public bool HasAnyMove()
		{
			return GetAllowedMoves().Count > 0;
		}

		private List<Move> GetSimpleMovesForPiece(int row, int col)
		{
			var piece = Board[row, col];
			var moves = new List<Move>();
			if (piece.IsKing)
			{
				int[] dr = { -1, -1, 1, 1 };
				int[] dc = { -1, 1, -1, 1 };
				for (int d = 0; d < 4; d++)
				{
					int r = row + dr[d];
					int c = col + dc[d];
					while (r >= 0 && r < 8 && c >= 0 && c < 8 && Board[r, c] == null)
					{
						moves.Add(new Move(row, col, r, c, false, null));
						r += dr[d];
						c += dc[d];
					}
				}
			}
			else
			{
				int dir = (piece.Player == Player.Yellow) ? 1 : -1;
				foreach (int dc in new[] { -1, 1 })
				{
					int r = row + dir;
					int c = col + dc;
					if (r >= 0 && r < 8 && c >= 0 && c < 8 && Board[r, c] == null)
					{
						moves.Add(new Move(row, col, r, c, false, null));
					}
						
				}
			}
			return moves;
		}

		private List<Move> GetCaptureMovesForPiece(int row, int col)
		{
			var piece = Board[row, col];
			var moves = new List<Move>();
			if (piece.IsKing)
			{
				int[] dr = { -1, -1, 1, 1 };
				int[] dc = { -1, 1, -1, 1 };
				for (int d = 0; d < 4; d++)
				{
					int r = row + dr[d];
					int c = col + dc[d];
					bool foundEnemy = false;
					int enemyRow = -1, enemyCol = -1;
					while (r >= 0 && r < 8 && c >= 0 && c < 8)
					{
						if (Board[r, c] != null)
						{
							if (!foundEnemy && Board[r, c].Player != piece.Player)
							{
								foundEnemy = true;
								enemyRow = r;
								enemyCol = c;
							}
							else break;
						}
						else if (foundEnemy)
						{
							moves.Add(new Move(row, col, r, c, true, new List<(int, int)> { (enemyRow, enemyCol) }));
						}
						r += dr[d];
						c += dc[d];
					}
				}
			}
			else
			{
				int[] dr = { -1, -1, 1, 1 };
				int[] dc = { -1, 1, -1, 1 };
				for (int i = 0; i < 4; i++)
				{
					int midRow = row + dr[i];
					int midCol = col + dc[i];
					int targetRow = row + 2 * dr[i];
					int targetCol = col + 2 * dc[i];
					if (targetRow >= 0 && targetRow < 8 && targetCol >= 0 && targetCol < 8)
					{
						var midPiece = Board[midRow, midCol];
						if (midPiece != null && midPiece.Player != piece.Player && Board[targetRow, targetCol] == null)
						{
							moves.Add(new Move(row, col, targetRow, targetCol, true, new List<(int, int)> { (midRow, midCol) }));
						}
					}
				}
			}
			return moves;
		}

		public bool MakeMove(Move move, out bool moreCapturesAvailable)
		{
			moreCapturesAvailable = false;

			List<Move> allowed = GetAllowedMoves();

			bool isValid = false;
			foreach (var m in allowed)
			{
				if (m.FromRow == move.FromRow && m.FromCol == move.FromCol && m.ToRow == move.ToRow && m.ToCol == move.ToCol)
				{
					isValid = true;
					break;
				}
			}
			if (!isValid) return false;

			Piece piece = Board[move.FromRow, move.FromCol];

			Board[move.ToRow, move.ToCol] = piece;
			Board[move.FromRow, move.FromCol] = null;

			if (move.IsCapture && move.CapturedPieces != null)
			{
				foreach (var (cr, cc) in move.CapturedPieces)
				{ 
					Board[cr, cc] = null;
				}
			}

			if (!piece.IsKing)
			{
				if (piece.Player == Player.Yellow && move.ToRow == 7 || piece.Player == Player.Red && move.ToRow == 0)
				{
					piece.IsKing = true;
				}
			}

			List<Move> additionalCaptures = GetCaptureMovesForPiece(move.ToRow, move.ToCol);
			if (move.IsCapture && additionalCaptures.Count > 0)
			{
				moreCapturesAvailable = true;
			}
			else
			{
				CurrentPlayer = (CurrentPlayer == Player.Yellow) ? Player.Red : Player.Yellow;
			}
			return true;
		}
	}
}