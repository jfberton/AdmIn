namespace AdmIn.UI.Services
{
    using AdmIn.Business.Entidades;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using iText.IO.Font.Constants;
    using iText.Kernel.Font;
    using System.Globalization;
    using System.Text;
    using iText.IO.Image;
    using iText.Layout.Borders;

    public class Reportes
    {
        private readonly IWebHostEnvironment _env;

        public Reportes(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<byte[]> GenerarPDFReserva(Reserva? rec)
        {
            byte[] ret = null;

            if (rec != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfWriter writer = new PdfWriter(ms))
                    {
                        using (PdfDocument pdfDoc = new PdfDocument(writer))
                        {
                            Document doc = new Document(pdfDoc);
                            doc.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));

                            #region Encabezado

                            Table table_encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 2, 8, 2 })).UseAllAvailableWidth();
                            table_encabezado.SetFontSize(8);

                            //creo una tabla para agregar la imagen el titulo y a quien va dirigido
                            string imagePath = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot\\img", "AdmIn.png");
                            iText.Layout.Element.Image logo = new iText.Layout.Element.Image(ImageDataFactory.Create(imagePath));
                            logo.ScaleToFit(80, 60);

                            // Crear una celda y agregar la imagen a la celda
                            Cell cell = new Cell(1, 1).Add(logo)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                            table_encabezado.AddCell(cell);

                            //creo la celda para agregar el titulo
                            cell = new Cell(2, 1).Add(new Paragraph("RESERVA INMUEBLE")).SimulateBold()
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFontSize(12)
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                            table_encabezado.AddCell(cell);

                            //creo otra tabla para agregar a quien va dirigodo y luego la inserto en la ultima celda de la tabla encabezado
                            Table otra_tabla = new Table(UnitValue.CreatePercentArray(new float[] { 1 })).UseAllAvailableWidth();
                            cell = new Cell(1, 1).Add(new Paragraph("FOLIO")).SimulateBold().SetTextAlignment(TextAlignment.CENTER).SetFontSize(8).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                            otra_tabla.AddCell(cell);
                            cell = new Cell(1, 1).Add(new Paragraph("XXXXX")).SetTextAlignment(TextAlignment.CENTER).SetFontSize(8).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                            otra_tabla.AddCell(cell);

                            cell = new Cell(1, 1).Add(otra_tabla).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                            table_encabezado.AddCell(cell);

                            doc.Add(table_encabezado);

                            #endregion


                            // Detalles de la reserva
                            doc.Add(new Paragraph($"Bueno por: {rec.CostoReserva:C}"));
                            doc.Add(new Paragraph($"Fecha de Reserva: {rec.FechaCreacion.ToString("d", CultureInfo.CurrentCulture)}").SetTextAlignment(TextAlignment.LEFT));

                            #region convierto el valor en letras
                            Numalet nl = new Numalet();
                            nl.SeparadorDecimalSalida = "pesos ";
                            nl.LetraCapital = true;
                            nl.MascaraSalidaDecimal = "00/100 M.N.";

                            string montoDocumentoString = "Son " + nl.ToCustomCardinal(rec.CostoReserva);
                            #endregion

                            doc.Add(new Paragraph($"Cantidad con letra: {montoDocumentoString}")).SetTextAlignment(TextAlignment.RIGHT);

                            doc.Add(new Paragraph($"Recibimos de {rec.Persona.Nombre} {rec.Persona.ApellidoPaterno} {rec.Persona.ApellidoMaterno}, " +
                                $"la cantidad arriba mencionada por concepto de pago de investigación y apartado hasta el día {rec.FechaFinalizacion.ToLongDateString()}" +
                                $" sobre una opción de casa en arrendamiento ubicada en {rec.Inmueble.Direccion.ToString()}. ").SetTextAlignment(TextAlignment.JUSTIFIED));

                            #region Condiciones adicionales

                            Table tablaCondiciones = new Table(UnitValue.CreatePercentArray(new float[] { 1, 9 })).UseAllAvailableWidth();
                            tablaCondiciones.SetFontSize(9);

                            // Lista de condiciones
                            var condiciones = new List<(string Numero, string Texto)>
                                {
                                    ("1)", "En caso de no salir satisfactorio el resultado de la evaluación, el importe pagado no es reembolsable. (Este importe es equivalente a $1,000 un mil pesos 00/100 M.N.)"),
                                    ("2)", "Si por contrario el resultado es positivo, se tomará a cuenta del depósito en garantía de la renta del inmueble o del pago de la póliza jurídica."),
                                    ("3)", "En caso de requerir factura, se deberá pagar el IVA correspondiente.")
                                };

                            foreach (var (numero, texto) in condiciones)
                            {
                                var cellNumero = new Cell().Add(new Paragraph(numero))
                                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                    .SetVerticalAlignment(VerticalAlignment.TOP)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetPaddingTop(2);

                                var cellTexto = new Cell().Add(new Paragraph(texto))
                                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                                    .SetPaddingBottom(5);

                                tablaCondiciones.AddCell(cellNumero);
                                tablaCondiciones.AddCell(cellTexto);
                            }

                            doc.Add(tablaCondiciones);

                            #endregion

                            // Cierre del documento
                            doc.Close();
                        }
                    }
                    ret = ms.ToArray();
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// Convierte números en su expresión numérica a su numeral cardinal
    /// </summary>
    public sealed class Numalet
    {
        /*
        Algunos ejemplos para usarla:

        MessageBox.Show(Numalet.ToCardinal("18,25"));
        //dieciocho con 25/100.-

        //Si tenemos el número en un string con otro separador decimal (por ejemplo punto):
        MessageBox.Show(Numalet.ToCardinal("155.38", new CultureInfo("en-US")));
        //ciento cincuenta y cinco con 38/100.-

        //instanciando la clase podemos generar salidas variadas
        Numalet let;
        let = new Numalet();
        //un porcentaje como he visto en algunos documentos de caracter legal:
        let.MascaraSalidaDecimal = "por ciento";
        let.SeparadorDecimalSalida = "con";
        let.ConvertirDecimales = true;
        MessageBox.Show(let.ToCustomCardinal(21.2));
        //veintiuno con veinte por ciento

        let = null;
        let = new Numalet();
        //al uso en México (creo):
        let.MascaraSalidaDecimal = "00/100 M.N.";
        let.SeparadorDecimalSalida = "pesos";
        //observar que sin esta propiedad queda "veintiuno pesos" en vez de "veintiún pesos":
        let.ApocoparUnoParteEntera = true;
        MessageBox.Show("Son: " + let.ToCustomCardinal(1121.24));
        //Son: un mil ciento veintiún pesos 24/100 M.N.

        //algo más raro
        let.MascaraSalidaDecimal = "###0 dracmas";
        //###0 quiere hace que se redondee a 4 decimales y no se muestren los ceros a la izquierda,
        //en cambio 0000 haría que, aparte de redondear en 4, se muestren los ceros a la izquierda.
        let.SeparadorDecimalSalida = "talentos y";
        let.LetraCapital = true;
        MessageBox.Show(let.ToCustomCardinal(12.085));
        //Doce talentos y 850 dracmas

        //una variación del anterior redondeando decimales a mano
        let.ConvertirDecimales = true;
        //redondeando en cuatro decimales
        let.Decimales = 4;
        let.MascaraSalidaDecimal = "dracmas";
        MessageBox.Show(let.ToCustomCardinal(21.50028354));
        //Veintiún talentos y cinco mil tres dracmas
        let = null;

        */

        #region Miembros estáticos

        private const int UNI = 0, DIECI = 1, DECENA = 2, CENTENA = 3;
        private static string[,] _matriz = new string[CENTENA + 1, 10]
        {
        {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
        {" diez"," once"," doce"," trece"," catorce"," quince"," dieciséis"," diecisiete"," dieciocho"," diecinueve"},
        {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
        {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
        };

        private const Char sub = (Char)26;
        //Cambiar acá si se quiere otro comportamiento en los métodos de clase
        public const String SeparadorDecimalSalidaDefault = "con";
        public const String MascaraSalidaDecimalDefault = "00'/100.-'";
        public const Int32 DecimalesDefault = 2;
        public const Boolean LetraCapitalDefault = false;
        public const Boolean ConvertirDecimalesDefault = false;
        public const Boolean ApocoparUnoParteEnteraDefault = false;
        public const Boolean ApocoparUnoParteDecimalDefault = false;

        #endregion

        #region Propiedades

        private Int32 _decimales = DecimalesDefault;
        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
        private Int32 _posiciones = DecimalesDefault;
        private String _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private Boolean _esMascaraNumerica = true;
        private Boolean _letraCapital = LetraCapitalDefault;
        private Boolean _convertirDecimales = ConvertirDecimalesDefault;
        private Boolean _apocoparUnoParteEntera = false;
        private Boolean _apocoparUnoParteDecimal;

        /// <summary>
        /// Indica la cantidad de decimales que se pasarán a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
        public Int32 Decimales
        {
            get { return _decimales; }
            set
            {
                if (value > 10) throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                _decimales = value;
            }
        }

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public String SeparadorDecimalSalida
        {
            get { return _separadorDecimalSalida; }
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                if (value.Trim().IndexOf(" ") > 0)
                    _apocoparUnoParteEntera = true;
                else _apocoparUnoParteEntera = false;
            }
        }

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public String MascaraSalidaDecimal
        {
            get
            {
                if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    return _mascaraSalidaDecimal;
                else return "";
            }
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                //que haya al principio de la cadena, y también si es una máscara numérica
                int i = 0;
                while (i < value.Length
                    && (value[i] == '0')
                    | value[i] == '#')
                    i++;
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else _esMascaraNumerica = false;
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'"
                        + value.Substring(_posiciones)
                            .Replace("''", sub.ToString())
                            .Replace("'", String.Empty)
                            .Replace(sub.ToString(), "'") + "'";
                else
                    _mascaraSalidaDecimalInterna = value
                        .Replace("''", sub.ToString())
                        .Replace("'", String.Empty)
                        .Replace(sub.ToString(), "'");
            }
        }

        /// <summary>
        /// Indica si la primera letra del resultado debe estár en mayúscula
        /// </summary>
        public Boolean LetraCapital
        {
            get { return _letraCapital; }
            set { _letraCapital = value; }
        }

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public Boolean ConvertirDecimales
        {
            get { return _convertirDecimales; }
            set
            {
                _convertirDecimales = value;
                _apocoparUnoParteDecimal = value;
                if (value)
                {// Si la máscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                        MascaraSalidaDecimal = "";
                }
                else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    //Si no hay máscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            }
        }

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public Boolean ApocoparUnoParteEntera
        {
            get { return _apocoparUnoParteEntera; }
            set { _apocoparUnoParteEntera = value; }
        }

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public Boolean ApocoparUnoParteDecimal
        {
            get { return _apocoparUnoParteDecimal; }
            set { _apocoparUnoParteDecimal = value; }
        }

        #endregion

        #region Constructores

        public Numalet()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public Numalet(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
        {
            if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
                this.MascaraSalidaDecimal = MascaraSalidaDecimal;
            if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
                _separadorDecimalSalida = SeparadorDecimalSalida;
            _letraCapital = LetraCapital;
            _convertirDecimales = ConvertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public String ToCustomCardinal(Double Numero)
        { return Convertir((Decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, _apocoparUnoParteDecimal); }

        public String ToCustomCardinal(String Numero)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
                return ToCustomCardinal(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public String ToCustomCardinal(Decimal Numero)
        { return ToCardinal((Numero)); }

        public String ToCustomCardinal(Int32 Numero)
        { return Convertir((Decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, false); }

        #endregion

        #region Conversores estáticos

        public static String ToCardinal(Int32 Numero)
        {
            return Convertir((Decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static String ToCardinal(Double Numero)
        {
            return ToCardinal((Decimal)Numero);
        }

        public static String ToCardinal(String Numero, CultureInfo ReferenciaCultural)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
                return ToCardinal(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public static String ToCardinal(String Numero)
        {
            return Numalet.ToCardinal(Numero, CultureInfo.CurrentCulture);
        }

        public static String ToCardinal(Decimal Numero)
        {
            return Convertir(Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        #endregion

        private static String Convertir(Decimal Numero, Int32 Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, Boolean EsMascaraNumerica, Boolean LetraCapital, Boolean ConvertirDecimales, Boolean ApocoparUnoParteEntera, Boolean ApocoparUnoParteDecimal)
        {
            Int64 Num;
            Int32 terna, centenaTerna, decenaTerna, unidadTerna, iTerna;
            String cadTerna;
            StringBuilder Resultado = new StringBuilder();

            Num = (Int64)Math.Abs(Numero);

            if (Num >= 1000000000000 || Num < 0) throw new ArgumentException("El número '" + Numero.ToString() + "' excedió los límites del conversor: [0;1.000.000.000.000)");
            if (Num == 0)
                Resultado.Append(" cero");
            else
            {
                iTerna = 0;
                while (Num > 0)
                {
                    iTerna++;
                    cadTerna = String.Empty;
                    terna = (Int32)(Num % 1000);

                    centenaTerna = (Int32)(terna / 100);
                    decenaTerna = terna % 100;
                    unidadTerna = terna % 10;

                    if ((decenaTerna > 0) && (decenaTerna < 10))
                        cadTerna = _matriz[UNI, unidadTerna] + cadTerna;
                    else if ((decenaTerna >= 10) && (decenaTerna < 20))
                        cadTerna = cadTerna + _matriz[DIECI, unidadTerna];
                    else if (decenaTerna == 20)
                        cadTerna = cadTerna + " veinte";
                    else if ((decenaTerna > 20) && (decenaTerna < 30))
                        cadTerna = " veinti" + _matriz[UNI, unidadTerna].Substring(1);
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                        if (unidadTerna != 0)
                            cadTerna = _matriz[DECENA, (Int32)(decenaTerna / 10)] + " y" + _matriz[UNI, unidadTerna] + cadTerna;
                        else
                            cadTerna += _matriz[DECENA, (Int32)(decenaTerna / 10)];

                    switch (centenaTerna)
                    {
                        case 1:
                            if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                            else cadTerna = " cien" + cadTerna;
                            break;
                        case 5:
                        case 7:
                        case 9:
                            cadTerna = _matriz[CENTENA, (Int32)(terna / 100)] + cadTerna;
                            break;
                        default:
                            if ((Int32)(terna / 100) > 1) cadTerna = _matriz[UNI, (Int32)(terna / 100)] + "cientos" + cadTerna;
                            break;
                    }
                    //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                    if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                        cadTerna = cadTerna.Replace("veintiuno", "veintiún");
                    else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    //Acentúo 'veintidós', 'veintitrés' y 'veintiséis'
                    else if (decenaTerna == 22) cadTerna = cadTerna.Replace("veintidos", "veintidós");
                    else if (decenaTerna == 23) cadTerna = cadTerna.Replace("veintitres", "veintitrés");
                    else if (decenaTerna == 26) cadTerna = cadTerna.Replace("veintiseis", "veintiséis");

                    //Completo miles y millones
                    switch (iTerna)
                    {
                        case 3:
                            if (Numero < 2000000) cadTerna += " millón";
                            else cadTerna += " millones";
                            break;
                        case 2:
                        case 4:
                            if (terna > 0) cadTerna += " mil";
                            break;
                    }
                    Resultado.Insert(0, cadTerna);
                    Num = (Int32)(Num / 1000);
                } //while
            }

            //Se agregan los decimales si corresponde
            if (Decimales > 0)
            {
                Resultado.Append(" " + SeparadorDecimalSalida + " ");
                Int32 EnteroDecimal = (Int32)Math.Round((Double)(Numero - (Int64)Numero) * Math.Pow(10, Decimales), 0);
                if (ConvertirDecimales)
                {
                    Boolean esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                    Resultado.Append(Convertir((Decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                        + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
                }
                else
                if (EsMascaraNumerica) Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                else Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
            }
            //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
            if (LetraCapital)
                return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
            else
                return Resultado.ToString().Substring(1);
        }
    }
}
