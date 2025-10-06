using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Snippets.Font;

using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public static class PdfWriter
{
    public static void WritePdf(Puzzle origPuzzle, Puzzle puzzle, Puzzle lastConsistentState, string fileName)
    {
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
        
        // Create a new PDF document.  
        var document = new PdfDocument();
        document.Info.Title = "Created with PDFsharp";
        document.Info.Subject = "Just a simple Hello-World program.";

        // Create an empty page in this document.
        var page = document.AddPage();

        // Get an XGraphics object for drawing on this page.
        var gfx = XGraphics.FromPdfPage(page);

       
        var distance = 30;
        var width = distance*9;
        var height = distance*9;
      
        var topLeft = new XPoint(50, 50);
        
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

        //output original values in bold font 
        
        // Create a font.
        var regularFont = new XFont("Times New Roman", 10, XFontStyleEx.Regular);
        var boldFont = new XFont("Times New Roman", 10, XFontStyleEx.Bold);

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
            }   
        }
        
        // Save the document...
        document.Save(fileName);
    }
    
    public static void WritePdfWithCandidates(Puzzle origPuzzle, Puzzle puzzle, Puzzle lastConsistentState, string fileName)
    {
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
        
        // Create a new PDF document.  
        var document = new PdfDocument();
        document.Info.Title = "Created with PDFsharp";
        document.Info.Subject = "Just a simple Hello-World program.";

        // Create an empty page in this document.
        var page = document.AddPage();

        // Get an XGraphics object for drawing on this page.
        var gfx = XGraphics.FromPdfPage(page);

       
        var distance = 30;
        var width = distance*9;
        var height = distance*9;
      
        var topLeft = new XPoint(50, 50);
        
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

        //output original values in bold font 
        
        // Create a font.
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
                else
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
        
        // Save the document...
        document.Save(fileName);
    }
}