using System.Drawing.Printing;
using System.Windows;

namespace POL.Lib.Utils
{
    public class HelperPrinter
    {
        public static Point GetPaperKindDimensions(PaperKind kind)
        {
            Point result;
            switch (kind)
            {
                case PaperKind.Custom:
                    result = new Point(0, 0);
                    break;

                case PaperKind.Letter:
                    result = Inches(8.5, 11);
                    break;
                case PaperKind.Legal:
                    result = Inches(8.5, 14);
                    break;
                case PaperKind.A4:
                    result = Millimeters(210, 297);
                    break;
                case PaperKind.CSheet:
                    result = Inches(17, 22);
                    break;
                case PaperKind.DSheet:
                    result = Inches(22, 34);
                    break;
                case PaperKind.ESheet:
                    result = Inches(34, 44);
                    break;
                case PaperKind.LetterSmall:
                    result = Inches(8.5, 11);
                    break;
                case PaperKind.Tabloid:
                    result = Inches(11, 17);
                    break;
                case PaperKind.Ledger:
                    result = Inches(17, 11);
                    break;
                case PaperKind.Statement:
                    result = Inches(5.5, 8.5);
                    break;
                case PaperKind.Executive:
                    result = Inches(7.25, 10.5);
                    break;
                case PaperKind.A3:
                    result = Millimeters(297, 420);
                    break;
                case PaperKind.A4Small:
                    result = Millimeters(210, 297);
                    break;
                case PaperKind.A5:
                    result = Millimeters(148, 210);
                    break;
                case PaperKind.B4:
                    result = Millimeters(250, 354);
                    break;
                case PaperKind.B5:
                    result = Millimeters(182, 257);
                    break;
                case PaperKind.Folio:
                    result = Inches(8.5, 13);
                    break;
                case PaperKind.Quarto:
                    result = Millimeters(215, 275);
                    break;
                case PaperKind.Standard10x14:
                    result = Inches(10, 14);
                    break;
                case PaperKind.Standard11x17:
                    result = Inches(11, 17);
                    break;
                case PaperKind.Note:
                    result = Inches(8.5, 11);
                    break;
                case PaperKind.Number9Envelope:
                    result = Inches(3.875, 8.875);
                    break;
                case PaperKind.Number10Envelope:
                    result = Inches(4.125, 9.5);
                    break;
                case PaperKind.Number11Envelope:
                    result = Inches(4.5, 10.375);
                    break;
                case PaperKind.Number12Envelope:
                    result = Inches(4.75, 11);
                    break;
                case PaperKind.Number14Envelope:
                    result = Inches(5, 11.5);
                    break;
                case PaperKind.DLEnvelope:
                    result = Millimeters(110, 220);
                    break;
                case PaperKind.C5Envelope:
                    result = Millimeters(162, 229);
                    break;
                case PaperKind.C3Envelope:
                    result = Millimeters(324, 458);
                    break;
                case PaperKind.C4Envelope:
                    result = Millimeters(229, 324);
                    break;
                case PaperKind.C6Envelope:
                    result = Millimeters(114, 162);
                    break;
                case PaperKind.C65Envelope:
                    result = Millimeters(114, 229);
                    break;
                case PaperKind.B4Envelope:
                    result = Millimeters(250, 353);
                    break;
                case PaperKind.B5Envelope:
                    result = Millimeters(176, 250);
                    break;
                case PaperKind.B6Envelope:
                    result = Millimeters(176, 125);
                    break;
                case PaperKind.ItalyEnvelope:
                    result = Millimeters(110, 230);
                    break;
                case PaperKind.MonarchEnvelope:
                    result = Inches(3.875, 7.5);
                    break;
                case PaperKind.PersonalEnvelope:
                    result = Inches(3.625, 6.5);
                    break;
                case PaperKind.USStandardFanfold:
                    result = Inches(14.875, 11);
                    break;
                case PaperKind.GermanStandardFanfold:
                    result = Inches(8.5, 12);
                    break;
                case PaperKind.GermanLegalFanfold:
                    result = Inches(8.5, 13);
                    break;

                case PaperKind.IsoB4:
                    result = Millimeters(250, 353);
                    break;
                case PaperKind.JapanesePostcard:
                    result = Millimeters(100, 148);
                    break;
                case PaperKind.Standard9x11:
                    result = Inches(9, 11);
                    break;
                case PaperKind.Standard10x11:
                    result = Inches(10, 11);
                    break;
                case PaperKind.Standard15x11:
                    result = Inches(15, 11);
                    break;
                case PaperKind.InviteEnvelope:
                    result = Millimeters(220, 220);
                    break;
                case PaperKind.LetterExtra:
                    result = Inches(9.275, 12);
                    break;
                case PaperKind.LegalExtra:
                    result = Inches(9.275, 15);
                    break;
                case PaperKind.TabloidExtra:
                    result = Inches(11.69, 18);
                    break;
                case PaperKind.A4Extra:
                    result = Inches(9.27, 12.69);
                    break;
                case PaperKind.LetterTransverse:
                    result = Inches(8.275, 11);
                    break;
                case PaperKind.A4Transverse:
                    result = Millimeters(210, 297);
                    break;
                case PaperKind.LetterExtraTransverse:
                    result = Inches(9.275, 12);
                    break;
                case PaperKind.APlus:
                    result = Millimeters(227, 356);
                    break;
                case PaperKind.BPlus:
                    result = Millimeters(305, 487);
                    break;
                case PaperKind.LetterPlus:
                    result = Inches(8.5, 12.69);
                    break;
                case PaperKind.A4Plus:
                    result = Millimeters(210, 330);
                    break;
                case PaperKind.A5Transverse:
                    result = Millimeters(148, 210);
                    break;
                case PaperKind.B5Transverse:
                    result = Millimeters(182, 257);
                    break;
                case PaperKind.A3Extra:
                    result = Millimeters(322, 445);
                    break;
                case PaperKind.A5Extra:
                    result = Millimeters(174, 235);
                    break;
                case PaperKind.B5Extra:
                    result = Millimeters(201, 276);
                    break;
                case PaperKind.A2:
                    result = Millimeters(420, 594);
                    break;
                case PaperKind.A3Transverse:
                    result = Millimeters(297, 420);
                    break;
                case PaperKind.A3ExtraTransverse:
                    result = Millimeters(322, 445);
                    break;

                case PaperKind.JapaneseDoublePostcard:
                    result = Millimeters(200, 148);
                    break;
                case PaperKind.A6:
                    result = Millimeters(105, 148);
                    break;
                case PaperKind.JapaneseEnvelopeKakuNumber2:
                    result = Millimeters(240, 332);
                    break;
                case PaperKind.JapaneseEnvelopeKakuNumber3:
                    result = Millimeters(216, 277);
                    break;
                case PaperKind.JapaneseEnvelopeChouNumber3:
                    result = Millimeters(120, 235);
                    break;
                case PaperKind.JapaneseEnvelopeChouNumber4:
                    result = Millimeters(90, 205);
                    break;
                case PaperKind.LetterRotated:
                    result = Inches(11, 8.5);
                    break;
                case PaperKind.A3Rotated:
                    result = Millimeters(420, 297);
                    break;
                case PaperKind.A4Rotated:
                    result = Millimeters(297, 210);
                    break;
                case PaperKind.A5Rotated:
                    result = Millimeters(210, 148);
                    break;
                case PaperKind.B4JisRotated:
                    result = Millimeters(364, 257);
                    break;
                case PaperKind.B5JisRotated:
                    result = Millimeters(257, 182);
                    break;
                case PaperKind.JapanesePostcardRotated:
                    result = Millimeters(148, 100);
                    break;
                case PaperKind.JapaneseDoublePostcardRotated:
                    result = Millimeters(148, 200);
                    break;
                case PaperKind.A6Rotated:
                    result = Millimeters(148, 105);
                    break;
                case PaperKind.JapaneseEnvelopeKakuNumber2Rotated:
                    result = Millimeters(332, 240);
                    break;
                case PaperKind.JapaneseEnvelopeKakuNumber3Rotated:
                    result = Millimeters(277, 216);
                    break;
                case PaperKind.JapaneseEnvelopeChouNumber3Rotated:
                    result = Millimeters(235, 120);
                    break;
                case PaperKind.JapaneseEnvelopeChouNumber4Rotated:
                    result = Millimeters(205, 90);
                    break;
                case PaperKind.B6Jis:
                    result = Millimeters(128, 182);
                    break;
                case PaperKind.B6JisRotated:
                    result = Millimeters(182, 128);
                    break;
                case PaperKind.Standard12x11:
                    result = Inches(12, 11);
                    break;
                case PaperKind.JapaneseEnvelopeYouNumber4:
                    result = Millimeters(105, 235);
                    break;
                case PaperKind.JapaneseEnvelopeYouNumber4Rotated:
                    result = Millimeters(235, 105);
                    break;
                case PaperKind.Prc16K:
                    result = Millimeters(146, 215);
                    break;
                case PaperKind.Prc32K:
                    result = Millimeters(97, 151);
                    break;
                case PaperKind.Prc32KBig:
                    result = Millimeters(97, 151);
                    break;
                case PaperKind.PrcEnvelopeNumber1:
                    result = Millimeters(102, 165);
                    break;
                case PaperKind.PrcEnvelopeNumber2:
                    result = Millimeters(102, 176);
                    break;
                case PaperKind.PrcEnvelopeNumber3:
                    result = Millimeters(125, 176);
                    break;
                case PaperKind.PrcEnvelopeNumber4:
                    result = Millimeters(110, 208);
                    break;
                case PaperKind.PrcEnvelopeNumber5:
                    result = Millimeters(110, 220);
                    break;
                case PaperKind.PrcEnvelopeNumber6:
                    result = Millimeters(120, 230);
                    break;
                case PaperKind.PrcEnvelopeNumber7:
                    result = Millimeters(160, 230);
                    break;
                case PaperKind.PrcEnvelopeNumber8:
                    result = Millimeters(120, 309);
                    break;
                case PaperKind.PrcEnvelopeNumber9:
                    result = Millimeters(229, 324);
                    break;
                case PaperKind.PrcEnvelopeNumber10:
                    result = Millimeters(324, 458);
                    break;
                case PaperKind.Prc16KRotated:
                    result = Millimeters(215, 146);
                    break;
                case PaperKind.Prc32KRotated:
                    result = Millimeters(151, 97);
                    break;
                case PaperKind.Prc32KBigRotated:
                    result = Millimeters(151, 97);
                    break;
                case PaperKind.PrcEnvelopeNumber1Rotated:
                    result = Millimeters(165, 102);
                    break;
                case PaperKind.PrcEnvelopeNumber2Rotated:
                    result = Millimeters(176, 102);
                    break;
                case PaperKind.PrcEnvelopeNumber3Rotated:
                    result = Millimeters(176, 125);
                    break;
                case PaperKind.PrcEnvelopeNumber4Rotated:
                    result = Millimeters(208, 110);
                    break;
                case PaperKind.PrcEnvelopeNumber5Rotated:
                    result = Millimeters(220, 110);
                    break;
                case PaperKind.PrcEnvelopeNumber6Rotated:
                    result = Millimeters(230, 120);
                    break;
                case PaperKind.PrcEnvelopeNumber7Rotated:
                    result = Millimeters(230, 160);
                    break;
                case PaperKind.PrcEnvelopeNumber8Rotated:
                    result = Millimeters(309, 120);
                    break;
                case PaperKind.PrcEnvelopeNumber9Rotated:
                    result = Millimeters(324, 229);
                    break;
                case PaperKind.PrcEnvelopeNumber10Rotated:
                    result = Millimeters(458, 324);
                    break;

                default:
                    result = new Point(0, 0);
                    break;
            }
            return result;
        }

        private static Point Inches(double width, double height)
        {
            const float conversion = 254;
            return new Point((int) (width*conversion), (int) (height*conversion));
        }

        private static Point Millimeters(double width, double height)
        {
            const float conversion = 10;
            return new Point((int) (width*conversion), (int) (height*conversion));
        }
    }
}
