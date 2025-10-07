using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public static class PdfWriter
{
    public static void WritePdf(string puzzleName, Puzzle origPuzzle, Puzzle lastConsistentState, bool withCandidates,
        string fileName)
    {
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

        var document = new Document();

        document.Info.Title = "Sudoku Puzzle";
        document.Info.Subject = "Created with Armin's Sudoku Solver";
        
        var section = document.AddSection();
        var paragraph = section.AddParagraph(puzzleName.Length!=0?puzzleName+"\n":"");

        var puzzleString = lastConsistentState.PrintCells("0");
        puzzleString = puzzleString.Replace("\n","");
        string sudokuWikiLink = "https://www.sudokuwiki.org/sudoku.htm?bd=" + puzzleString;
        var hyperlink = paragraph.AddHyperlink(sudokuWikiLink, HyperlinkType.Url);
        hyperlink.AddText("Open puzzle in the SudokuWiki.org solver.");

        var pdfRenderer = new PdfDocumentRenderer { Document = document };
        pdfRenderer.RenderDocument();
        
        var page = pdfRenderer.PdfDocument.Pages[0]; 
        var gfx = XGraphics.FromPdfPage(page);
        DrawSudoku(gfx, origPuzzle, lastConsistentState, withCandidates);
        
        pdfRenderer.Save(fileName);
    }

    private static void DrawSudoku(XGraphics gfx, Puzzle origPuzzle, Puzzle lastConsistentState, bool withCandidates)
    {
        var distance = 30;
        var width = distance*9;
        var height = distance*9;
      
        var topLeft = new XPoint(75, 120);
        
        var thinLine = new XPen(XColors.Black, 1);
        var thickLine = new XPen(XColors.Black, 3);
        
        for (var row=0; row<10; row++)
        {
            gfx.DrawLine((row%3==0)?thickLine:thinLine, topLeft.X, row*distance+topLeft.Y, topLeft.X+height, row*distance+topLeft.Y);
        }

        for (var column=0; column<10; column++)
        {
            gfx.DrawLine((column%3==0)?thickLine:thinLine, column*distance+topLeft.X, topLeft.Y, column*distance+topLeft.X, topLeft.Y+width);
        }

        var regularFont = new XFont("Times New Roman", 10, XFontStyleEx.Regular);
        var boldFont = new XFont("Times New Roman", 10, XFontStyleEx.Bold);

        var smallFont = new XFont("Times New Roman", 5, XFontStyleEx.Regular);
        
        foreach(var row in AllRows)
        {
            foreach(var column in AllColumns) 
            {
                Position position = new Position(row, column);
                var value = lastConsistentState.GetCellValue(position);
                if (value != 0)
                {
                    var origValue = origPuzzle.GetCellValue(position);
                    var brush = value == origValue ? XBrushes.Red : XBrushes.Black;
                    var font = value == origValue ? boldFont : regularFont;
                    
                    gfx.DrawString(value.ToString(), font, brush, 
                        new XRect(topLeft.X+(column-1)*distance, topLeft.Y+(row-1)*distance,
                            distance, distance), XStringFormats.Center);
                }
                else if (withCandidates)
                {
                    var candidates = lastConsistentState.GetCandidates(position);
                    string c = "";
                    for (var i = 0; i < 9; i++)
                    {
                        if (candidates[i])
                            c+=(i+1).ToString();
                    }

                    gfx.DrawString(c, smallFont, XBrushes.Black, 
                        new XRect(topLeft.X+(column-1)*distance, topLeft.Y+(row-1)*distance,
                            distance, distance), XStringFormats.Center);
                }
            }   
        }
    }
}