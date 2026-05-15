using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp2
{
	public partial class Form1 : Form
	{
		private Game game;
		private PictureBox[,] picBoard = new PictureBox[8, 8];
		private int? selectedRow = null;
		private int? selectedCol = null;
		private bool waitingForCaptureContinuation = false;
		private bool gameOver = false;

		private string yellowImagePath = @"C:\Users\maxgs\OneDrive\Desktop\yellow.png";
		private string yellowKingImagePath = @"C:\Users\maxgs\OneDrive\Desktop\yellow_king.png";
		private string redImagePath = @"C:\Users\maxgs\OneDrive\Desktop\red1.png";
		private string redKingImagePath = @"C:\Users\maxgs\OneDrive\Desktop\red_king.png";

		public Form1()
		{
			InitializeComponent();

			picBoard[0, 1] = pictureBox1; picBoard[0, 3] = pictureBox3; picBoard[0, 5] = pictureBox5; picBoard[0, 7] = pictureBox7;
			picBoard[1, 0] = pictureBox8; picBoard[1, 2] = pictureBox10; picBoard[1, 4] = pictureBox12; picBoard[1, 6] = pictureBox14;
			picBoard[2, 1] = pictureBox17; picBoard[2, 3] = pictureBox19; picBoard[2, 5] = pictureBox21; picBoard[2, 7] = pictureBox23;
			picBoard[3, 0] = pictureBox24; picBoard[3, 2] = pictureBox26; picBoard[3, 4] = pictureBox28; picBoard[3, 6] = pictureBox30;
			picBoard[4, 1] = pictureBox33; picBoard[4, 3] = pictureBox35; picBoard[4, 5] = pictureBox37; picBoard[4, 7] = pictureBox39;
			picBoard[5, 0] = pictureBox40; picBoard[5, 2] = pictureBox42; picBoard[5, 4] = pictureBox44; picBoard[5, 6] = pictureBox46;
			picBoard[6, 1] = pictureBox49; picBoard[6, 3] = pictureBox51; picBoard[6, 5] = pictureBox53; picBoard[6, 7] = pictureBox55;
			picBoard[7, 0] = pictureBox56; picBoard[7, 2] = pictureBox58; picBoard[7, 4] = pictureBox60; picBoard[7, 6] = pictureBox62;

			game = new Game();
			UpdateBoardUI();
			UpdateTurnLabel();
		}

		private void OnPictureBoxClick(int row, int col)
		{
			if (gameOver) return;

			if (waitingForCaptureContinuation)
			{
				if (selectedRow.HasValue && selectedCol.HasValue)
				{
					AttemptMove(selectedRow.Value, selectedCol.Value, row, col);
				}
				return;
			}

			var piece = game.Board[row, col];
			if (piece != null && piece.Player == game.CurrentPlayer)
			{
				ClearSelection();
				selectedRow = row;
				selectedCol = col;
				HighlightCell(row, col, true);
			}
			else if (selectedRow.HasValue && selectedCol.HasValue)
			{
				AttemptMove(selectedRow.Value, selectedCol.Value, row, col);
			}
		}

		private void AttemptMove(int fromRow, int fromCol, int toRow, int toCol)
		{
			var allowedMoves = game.GetAllowedMoves();
			Move validMove = null;
			foreach (var m in allowedMoves)
			{
				if (m.FromRow == fromRow && m.FromCol == fromCol && m.ToRow == toRow && m.ToCol == toCol)
				{
					validMove = m;
					break;
				}
			}

			if (validMove == null)
			{
				if (!waitingForCaptureContinuation)
				{
					ClearSelection();
				}
				return;
			}

			bool moreCaptures;
			if (game.MakeMove(validMove, out moreCaptures))
			{
				UpdateBoardUI();

				if (moreCaptures)
				{
					waitingForCaptureContinuation = true;
					ClearSelection();
					selectedRow = toRow;
					selectedCol = toCol;
					HighlightCell(toRow, toCol, true);
				}
				else
				{
					waitingForCaptureContinuation = false;
					ClearSelection();
					UpdateTurnLabel();

					if (!game.HasAnyMove())
					{
						gameOver = true;
						string winner = (game.CurrentPlayer == Player.Yellow) ? "Красные" : "Жёлтые";
						MessageBox.Show($"Игра окончена! Победили {winner}.", "Конец игры");
					}
				}
			}
			else
			{
				if (!waitingForCaptureContinuation)
				{
					ClearSelection();
				}
					
			}
		}

		private void UpdateBoardUI()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					var pb = picBoard[i, j];
					if (pb == null) continue;
					var piece = game.Board[i, j];
					if (piece == null)
					{
						pb.Image = null;
					}
					else
					{
						string path;
						if (piece.Player == Player.Yellow)
						{
							if (piece.IsKing)
							{
								path = yellowKingImagePath;
							}
							else
							{
								path = yellowImagePath;
							}
								
						}
						else
						{
							if (piece.IsKing)
							{
								path = redKingImagePath;
							}
							else
							{
								path = redImagePath;
							}	
						}
						try { pb.Image = Image.FromFile(path); }
						catch { pb.Image = null; }
					}
				}

			}
				
		}

		private void UpdateTurnLabel()
		{
			labelTurn.Text = $"Ход: {(game.CurrentPlayer == Player.Yellow ? "Жёлтые" : "Красные")}";
		}

		private void HighlightCell(int row, int col, bool highlight)
		{
			if (highlight)
			{ 
				picBoard[row, col].BorderStyle = BorderStyle.Fixed3D;
			}
			else
			{
				picBoard[row, col].BorderStyle = BorderStyle.FixedSingle;
			}
				
		}

		private void ClearSelection()
		{
			if (selectedRow.HasValue && selectedCol.HasValue)
			{
				HighlightCell(selectedRow.Value, selectedCol.Value, false);
			}
			selectedRow = null;
			selectedCol = null;
		}

		private void pictureBox1_Click(object sender, EventArgs e) { OnPictureBoxClick(0, 1); }
		private void pictureBox3_Click(object sender, EventArgs e) { OnPictureBoxClick(0, 3); }
		private void pictureBox5_Click(object sender, EventArgs e) { OnPictureBoxClick(0, 5); }
		private void pictureBox7_Click(object sender, EventArgs e) { OnPictureBoxClick(0, 7); }

		private void pictureBox8_Click(object sender, EventArgs e) { OnPictureBoxClick(1, 0); }
		private void pictureBox10_Click(object sender, EventArgs e) { OnPictureBoxClick(1, 2); }
		private void pictureBox12_Click(object sender, EventArgs e) { OnPictureBoxClick(1, 4); }
		private void pictureBox14_Click(object sender, EventArgs e) { OnPictureBoxClick(1, 6); }

		private void pictureBox17_Click(object sender, EventArgs e) { OnPictureBoxClick(2, 1); }
		private void pictureBox19_Click(object sender, EventArgs e) { OnPictureBoxClick(2, 3); }
		private void pictureBox21_Click(object sender, EventArgs e) { OnPictureBoxClick(2, 5); }
		private void pictureBox23_Click(object sender, EventArgs e) { OnPictureBoxClick(2, 7); }

		private void pictureBox24_Click(object sender, EventArgs e) { OnPictureBoxClick(3, 0); }
		private void pictureBox26_Click(object sender, EventArgs e) { OnPictureBoxClick(3, 2); }
		private void pictureBox28_Click(object sender, EventArgs e) { OnPictureBoxClick(3, 4); }
		private void pictureBox30_Click(object sender, EventArgs e) { OnPictureBoxClick(3, 6); }

		private void pictureBox33_Click(object sender, EventArgs e) { OnPictureBoxClick(4, 1); }
		private void pictureBox35_Click(object sender, EventArgs e) { OnPictureBoxClick(4, 3); }
		private void pictureBox37_Click(object sender, EventArgs e) { OnPictureBoxClick(4, 5); }
		private void pictureBox39_Click(object sender, EventArgs e) { OnPictureBoxClick(4, 7); }

		private void pictureBox40_Click(object sender, EventArgs e) { OnPictureBoxClick(5, 0); }
		private void pictureBox42_Click(object sender, EventArgs e) { OnPictureBoxClick(5, 2); }
		private void pictureBox44_Click(object sender, EventArgs e) { OnPictureBoxClick(5, 4); }
		private void pictureBox46_Click(object sender, EventArgs e) { OnPictureBoxClick(5, 6); }

		private void pictureBox49_Click(object sender, EventArgs e) { OnPictureBoxClick(6, 1); }
		private void pictureBox51_Click(object sender, EventArgs e) { OnPictureBoxClick(6, 3); }
		private void pictureBox53_Click(object sender, EventArgs e) { OnPictureBoxClick(6, 5); }
		private void pictureBox55_Click(object sender, EventArgs e) { OnPictureBoxClick(6, 7); }

		private void pictureBox56_Click(object sender, EventArgs e) { OnPictureBoxClick(7, 0); }
		private void pictureBox58_Click(object sender, EventArgs e) { OnPictureBoxClick(7, 2); }
		private void pictureBox60_Click(object sender, EventArgs e) { OnPictureBoxClick(7, 4); }
		private void pictureBox62_Click(object sender, EventArgs e) { OnPictureBoxClick(7, 6); }
	}
}