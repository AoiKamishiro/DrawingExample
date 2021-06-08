/*
 * 
 * Copyright (c) 2021 AoiKamishiro
 * Released under the MIT license
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the “Software”), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;

namespace CalendarBuilderSample
{
    internal static class Program
    {
        private const int PADDING = 54; //Padding
        private const int TARGET_HEIGHT = 3508; //A4 Height 
        private const int TARGET_WIDTH = 2480; //A4 Height 
        private const int TEMP_HEIGHT = 21000; //Tempolary Height 
        private const int SLIDER_WIDTH = 120; //Slider Width
        private const int CONTENT_WIDTH = TARGET_WIDTH - SLIDER_WIDTH - 3 * PADDING;
        private const int CONTENT_TIME_WIDTH = 350;
        private const int HEADER_HEIGHT = 1503; //Header image height
        private const int DATE_HEADER_HEIGHT = 160;
        private const int DATE_HEADER_OFFSET_X = 845;
        private const int DATE_HEADER_OFFSET_Y = 35;
        private const int TIME_OFFSET_X = 15;
        private const int DETAIL_OFFSET = PADDING + CONTENT_TIME_WIDTH + 20;
        private const int FOOTER_WIDTH = TARGET_WIDTH - 2 * PADDING;
        private const int FOOTER_HEIGHT = 200;
        private const PixelFormat PIXCEL_FORMAT = PixelFormat.Format32bppArgb;
        private const GraphicsUnit GRAPHICS_UNIT = GraphicsUnit.Pixel;

        #region Settings
        //In actual operation, read and specify the Json file for configuration.
        private static readonly string FontName = "MS UI Gothic"; //The name of the font installed on the computer where you want to run it.

        private static readonly SolidBrush BrushCalendarBG = new SolidBrush(Color.FromArgb(37, 237, 255));
        private static readonly SolidBrush BrushDateArea = new SolidBrush(Color.FromArgb(245, 255, 115));
        private static readonly SolidBrush BrushTimeArea = new SolidBrush(Color.FromArgb(153, 76, 0));
        private static readonly SolidBrush BrushEventArea = new SolidBrush(Color.FromArgb(255, 255, 255));
        private static readonly SolidBrush BrushFooterArea = new SolidBrush(Color.FromArgb(205, 255, 209));
        private static readonly SolidBrush BrushBoundary = new SolidBrush(Color.FromArgb(30, 30, 30));
        private static readonly SolidBrush BrushSliderArea = new SolidBrush(Color.FromArgb(185, 110, 209));
        private static readonly SolidBrush BrushDateText = new SolidBrush(Color.Black);
        private static readonly SolidBrush BrushTimeText = new SolidBrush(Color.White);
        private static readonly SolidBrush BrushEventUpcoming = new SolidBrush(Color.Black);
        private static readonly SolidBrush BrushEventFinished = new SolidBrush(Color.Black);

        private static string footerTex1 = "Created by AoiKamishiro";
        private static string footerTex2 = "Twitter: @aoi3192";
        private static string footerTex3 = "Github: https://github.com/AoiKamishiro";
        private static string footerTex4 = "";
        #endregion
        private static void Main(string[] args)
        {
            Console.WriteLine("Calendar Builder v3.2");
            List<CalendarEvent> calendarEvents = JsonSerializer.Deserialize<List<CalendarEvent>>(File.ReadAllText(@"EventJson.json"));
            Stream headerStream = new FileStream(@"Header.png", FileMode.Open);
            CreateCalendar(headerStream, calendarEvents, @"output.png");
            try
            {
                //Open File
                Process.Start("explorer.exe", Path.GetFullPath(@"output.png"));
            }
            catch
            {

            }
            Console.WriteLine("Output to \"" + Path.GetFullPath(@"output.png") + "\".");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        private static void CreateCalendar(Stream headerStream, List<CalendarEvent> calendarEvents, string savePath)
        {
            using Bitmap bitmap = new Bitmap(TARGET_WIDTH, TEMP_HEIGHT, PIXCEL_FORMAT);
            using Graphics graphics = Graphics.FromImage(bitmap);
            using ImageAttributes wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.Clamp);

            int y = 0;

            //BackGround
            graphics.FillRectangle(BrushCalendarBG, new Rectangle(0, 0, TARGET_WIDTH, TEMP_HEIGHT));

            //SliderBackground
            graphics.FillRectangle(BrushSliderArea, new Rectangle(CONTENT_WIDTH + PADDING * 2, PADDING, SLIDER_WIDTH, TEMP_HEIGHT - PADDING));

            //HeaderImage            
            y += PADDING;
            using Bitmap headerBmp = new Bitmap(headerStream);
            graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, CONTENT_WIDTH, HEADER_HEIGHT));
            graphics.DrawImage(headerBmp, new Rectangle(PADDING + 5, y + 5, CONTENT_WIDTH - 10, HEADER_HEIGHT - 10), 0, 0, headerBmp.Width, headerBmp.Height, GRAPHICS_UNIT, wrapMode);
            y += HEADER_HEIGHT;

            //ContentBackground
            y += PADDING;
            graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, CONTENT_WIDTH, TEMP_HEIGHT - HEADER_HEIGHT - 2 * PADDING));
            graphics.FillRectangle(BrushEventArea, new Rectangle(PADDING + 5, y + 5, CONTENT_WIDTH - 10, TEMP_HEIGHT - HEADER_HEIGHT - 2 * PADDING - 10));
            graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, CONTENT_TIME_WIDTH, TEMP_HEIGHT - HEADER_HEIGHT - 2 * PADDING));
            graphics.FillRectangle(BrushTimeArea, new Rectangle(PADDING + 5, y + 5, CONTENT_TIME_WIDTH - 10, TEMP_HEIGHT - HEADER_HEIGHT - 2 * PADDING - 10));

            //Content
            using Font detailFont = new Font(FontName, 50);
            using Font dateFont = new Font(FontName, 75);
            using Font timeFont = new Font(FontName, 40);
            using Font timeFontStart = new Font(FontName, 50);
            using Font timeFontEnd = new Font(FontName, 35);
            string date = "";
            string time = "";

            foreach (CalendarEvent calendarEvent in calendarEvents)
            {
                int localX = PADDING;
                int localY = y + 20;

                //Draw Date Header
                if (date != calendarEvent.GetDateString())
                {
                    graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, CONTENT_WIDTH, DATE_HEADER_HEIGHT));
                    graphics.FillRectangle(BrushDateArea, new Rectangle(PADDING + 5, y + 5, CONTENT_WIDTH - 10, DATE_HEADER_HEIGHT - 10));
                    graphics.DrawString(calendarEvent.GetDateString(), dateFont, BrushDateText, localX + DATE_HEADER_OFFSET_X, y + DATE_HEADER_OFFSET_Y);
                    date = calendarEvent.GetDateString();
                    y += DATE_HEADER_HEIGHT + 5;
                    time = "";
                }

                //Draw Event Time
                localX = PADDING + TIME_OFFSET_X;

                if (time != "" && time != calendarEvent.GetStartTimeString().Substring(0, 2))
                {
                    graphics.DrawLine2(new Pen(BrushBoundary, 5), PADDING, PADDING + CONTENT_TIME_WIDTH, y, 20, 10);
                }

                char[] charsStart = calendarEvent.GetStartTimeString().ToCharArray();
                char[] charsEnd = calendarEvent.GetEndTimeString().ToCharArray();
                for (int i = 0; i < charsStart.Length; i++)
                {
                    graphics.DrawString(charsStart[i].ToString(), timeFontStart, BrushTimeText, localX, y + 6);
                    localX += GetFontWidth(charsStart[i].ToString(), timeFontStart);
                }
                graphics.DrawString("～", timeFont, BrushTimeText, localX, y + 10);
                localX += GetFontWidth("～".ToString(), timeFont);
                for (int i = 0; i < charsEnd.Length; i++)
                {
                    graphics.DrawString(charsEnd[i].ToString(), timeFontEnd, BrushTimeText, localX, y + 15);
                    localX += GetFontWidth(charsEnd[i].ToString(), timeFontEnd);
                }
                graphics.DrawString("▶", timeFont, BrushBoundary, PADDING + CONTENT_TIME_WIDTH - 15, y + 15);

                //Draw Event Content
                localX = DETAIL_OFFSET;

                SolidBrush brush = (calendarEvent.EndTime < DateTime.Now) ? BrushEventUpcoming : BrushEventFinished;
                char[] vs = calendarEvent.Summary.ToCharArray();
                for (int i = 0; i < vs.Length; i++)
                {
                    graphics.DrawString(vs[i].ToString(), detailFont, brush, localX, y + 10);

                    localX += GetFontWidth(vs[i].ToString(), detailFont);
                    //NextLine
                    if (localX > CONTENT_WIDTH - 50 && i != vs.Length - 1)
                    {
                        localX = DETAIL_OFFSET;
                        y += 90;
                    }
                }
                y += 90;
                time = calendarEvent.GetStartTimeString().Substring(0, 2);
            }

            //Paddig
            graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, CONTENT_WIDTH, 10));
            y += 5;
            graphics.FillRectangle(BrushCalendarBG, new Rectangle(0, y, TARGET_WIDTH, TEMP_HEIGHT - y));
            y += PADDING;

            //Draw Footer
            graphics.FillRectangle(BrushBoundary, new Rectangle(PADDING, y, FOOTER_WIDTH, FOOTER_HEIGHT));
            graphics.FillRectangle(BrushFooterArea, new Rectangle(PADDING + 5, y + 5, FOOTER_WIDTH - 10, FOOTER_HEIGHT - 10));

            using Font font1 = new Font(FontName, 30);
            graphics.DrawString(
                footerTex1 + Environment.NewLine + footerTex2 + Environment.NewLine + footerTex3 + Environment.NewLine + footerTex4,
                font1,
                Brushes.Black,
                PADDING + 10,
                y + 20
                );

            using Font font2 = new Font(FontName, 20);
            graphics.DrawString("Generated(JST) " + DateTime.UtcNow.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"), font2, Brushes.Black, PADDING + 1900, y + 160);
            y += FOOTER_HEIGHT + PADDING;

            using Bitmap bitmap1 = new Bitmap(TARGET_WIDTH, y);
            using Graphics graphics1 = Graphics.FromImage(bitmap1);
            graphics1.DrawImage(bitmap, new Rectangle(0, 0, TARGET_WIDTH, y), new Rectangle(0, 0, TARGET_WIDTH, y), GRAPHICS_UNIT);

            //Resize
            bitmap1.Save(savePath);
        }
        /// <summary>
        /// Get Font Width.
        /// </summary>
        /// <param name="str">The character whose width you want to check. </param>
        /// <param name="font">Character font. </param>
        /// <returns></returns>
        private static int GetFontWidth(string str, Font font)
        {
            using Bitmap bitmap = new Bitmap(500, 500);
            using Graphics graphics = Graphics.FromImage(bitmap);
            using StringFormat stringFormat = new StringFormat();

            CharacterRange[] characterRanges = { new CharacterRange(0, 1) };
            stringFormat.SetMeasurableCharacterRanges(characterRanges);
            RectangleF rectangleF = new RectangleF(0, 0, 500, 500);
            Region[] regions = graphics.MeasureCharacterRanges(str, font, rectangleF, stringFormat);

            int width = (int)MathF.Round(regions[0].GetBounds(graphics).Width);

            //Temporary treatment. "Space" will be 1.
            if (width == 1)
            {
                width = Convert.ToInt32(font.Size / 2);
            }
            return width;
        }
        /// <summary>
        /// Method for drawing a dotted line
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="startX"></param>
        /// <param name="endX"></param>
        /// <param name="y"></param>
        /// <param name="onLength"></param>
        /// <param name="offLength"></param>
        private static void DrawLine2(this Graphics graphics, Pen pen, int startX, int endX, int y, int onLength, int offLength)
        {
            int x = startX;
            while (x < endX)
            {
                if (x + onLength < endX)
                {
                    graphics.DrawLine(pen, new Point(x, y), new Point(x + onLength, y));
                    x += onLength + offLength;
                }
                else
                {
                    graphics.DrawLine(pen, new Point(x, y), new Point(endX, y));
                    x = endX;
                }
            }
            return;
        }
    }
}
