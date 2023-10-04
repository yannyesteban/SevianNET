namespace Whendy;

public struct Item
{
    public Item(int pos, string value, int priority, Token type)
    {
        this.Pos = pos;
        this.Value = value;
        this.Priority = priority;
        this.Type = type;

    }
    public int Pos;
    public string Value;
    public int Priority;
    public Token Type;



}

public class Lexer
{

    readonly string input;
    int pos = 0;
    char ch = '\0';
    int rd = 0;
    private bool markEOL;



    public Lexer(string input)
    {
        this.input = input;

        pos = 0;
        rd = 0;
        ch = ' ';
        eof = false;
        markEOL = false;
        Next();
    }

    private void error(string msg)
    {
        throw new SystemException();
    }

    private Token evalOp(char ch, Token tokenDefault, Token tokenAssign, Token tokenX2, Token tokenX3)
    {
        if (this.ch == '=')
        {
            Next();
            return tokenAssign;
        }
        if (tokenX2 != Token.ILLEGAL && this.ch == ch)
        {
            Next();
            if (tokenX3 != Token.ILLEGAL && this.ch == ch)
            {
                Next();
                return tokenX3;
            }
            return tokenX2;
        }
        return tokenDefault;
    }

    private Token doubleOp(char ch, Token tokenDefault, Token tokenX2)
    {
        if (tokenX2 != Token.ILLEGAL && this.ch == ch)
        {
            Next();
            return tokenX2;
        }
        return tokenDefault;
    }
    private bool IsLetter(char ch)
    {
        if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
        {
            return true;
        }
        return false;
    }

    private bool isAlphaNumeric(char ch)
    {
        if (
          (ch >= 'a' && ch <= 'z') ||
          (ch >= 'A' && ch <= 'Z') ||
          (ch >= '0' && ch <= '9') ||
          ch == '_'
        )
        {
            return true;
        }
        return false;
    }

    private bool IsDecimal(char ch)
    {
        return ch >= '0' && ch <= '9';
    }

    private bool IsHex(char ch)
    {
        return '0' <= ch && ch <= '9' || 'a' <= char.ToLower(ch) && char.ToLower(ch) <= 'f';
    }

    private void SkipWhitespace()
    {

        while (ch == ' ' || ch == '\t' || ch == '\n' && !markEOL || ch == '\r')
        {
            Next();
        }
    }

    string scanIdentifier()
    {
        int init = this.pos;
        int end = init;
        string lit = "";
        while (!eof)
        {
            var ch = this.ch;

            if (isAlphaNumeric(ch))
            {

                end++;
                lit += this.ch;

                Next();
                continue;

            }

            break;
        }

        return lit;
    }

    private void Digits(int _base)
    {
        if (_base <= 10)
        {
            //max := rune('0' + base)
            while (IsDecimal(this.ch))
            {

                Next();
            }
        }
        else
        {
            while (IsHex(this.ch))
            {

                Next();
            }
        }

    }
    private (string, Token) ScanNumber()
    {
        int offs = this.pos;
        Token tok = Token.INT;

        int _base = 10;        // number base
        char prefix = '0'; // one of 0 (decimal), '0' (0-octal), 'x', 'o', or 'b'
        //digsep = 0       // bit 0: digit present, bit 1: '_' present
        //invalid = -1     // index of invalid digit in literal, or < 0

        string lit = "c";
        // integer part
        if (this.ch != '.')
        {
            tok = Token.INT;
            if (this.ch == '0')
            {
                Next();
                switch (char.ToLower(ch))
                {
                    case 'x':
                        Next();
                        (_base, prefix) = (16, 'x');
                        break;
                    case 'o':
                        Next();
                        (_base, prefix) = (8, 'o');

                        break;
                    case 'b':
                        Next();
                        (_base, prefix) = (2, 'b');

                        break;
                    default:
                        (_base, prefix) = (8, '0');
                        break;

                }
            }
            //digsep |= 
            Digits(_base);
        }

        // fractional part
        if (this.ch == '.')
        {
            tok = Token.FLOAT;
            if (prefix == 'o' || prefix == 'b')
            {
                //s.error(s.offset, "invalid radix point in "+litname(prefix))
            }
            Next();
            Digits(_base);

        }
        /*
        if digsep&1 == 0 {
            s.error(s.offset, litname(prefix)+" has no digits")
        }
        */
        // exponent
        char e = char.ToLower(ch);
        if (e == 'e' || e == 'p')
        {
            /*
            switch {
            case e == 'e' && prefix != 0 && prefix != '0':
                s.errorf(s.offset, "%q exponent requires decimal mantissa", s.ch)
            case e == 'p' && prefix != 'x':
                s.errorf(s.offset, "%q exponent requires hexadecimal mantissa", s.ch)
            }
            */
            Next();
            tok = Token.FLOAT;
            if (this.ch == '+' || this.ch == '-')
            {
                Next();
            }
            //let ds = this.Digits(10)
            /*digsep |= ds
            if ds&1 == 0 {
                s.error(s.offset, "exponent has no digits")
            }
            */
        }
        /*
        else if prefix == 'x' && tok == token.FLOAT {
            s.error(s.offset, "hexadecimal mantissa requires a 'p' exponent")
        }
    
        // suffix 'i'
        if s.ch == 'i' {
            tok = token.IMAG
            s.next()
        }
    
        lit = string(s.src[offs:s.offset])
        if tok == token.INT && invalid >= 0 {
            s.errorf(invalid, "invalid digit %q in %s", lit[invalid-offs], litname(prefix))
        }
        if digsep&2 != 0 {
            if i = invalidSep(lit); i >= 0 {
                s.error(offs+i, "'_' must separate successive digits")
            }
        }
        */
        //lit = this.input.substring(offs, this.pos);
        lit = this.input[offs..this.pos];
        //console.log("<>", offs, this.pos, " = ", lit)
        //return lit
        return (lit, tok);
    }

    int digitVal(char ch)
    {

        if ('0' <= ch && ch <= '9')
        {

            return (int)ch - (int)'0';
        }
        if ('a' <= char.ToLower(ch) && char.ToLower(ch) <= 'f')
        {
            return (int)ch - (int)'a' + 10;
        }

        return 16;
    }
    private bool scanEscape(char quote)
    {
        int offs = this.pos;

        int n;
        int _base, max;

        if (this.ch == quote)
        {
            Next();
            return true;
        }
        switch (this.ch)
        {
            case 'a':
            case 'b':
            case 'f':
            case 'n':
            case 'r':
            case 't':
            case 'v':
            case '\\':
                //case quote:
                Next();
                return true;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
                (n, _base, max) = (3, 8, 255);
                break;
            case 'x':
                Next();
                (n, _base, max) = (2, 16, 255);
                break;
            case 'u':
                Next();
                (n, _base, max) = (4, 16, 99999999 /*unicode.MaxRune*/);
                break;
            case 'U':
                Next();
                (n, _base, max) = (8, 16, 99999999 /*unicode.MaxRune*/);
                break;
            default:
                string msg = "unknown escape sequence";
                if (this.ch < '\0')
                {
                    msg = "escape sequence not terminated";
                }
                this.error(msg);
                return false;
        }

        int x = 0;
        while (n > 0)
        {
            int d = this.digitVal(this.ch);
            if (d >= _base)
            {
                string msg = "illegal character %#U in escape sequence" + this.ch;
                if (this.ch < '\n')
                {
                    msg = "escape sequence not terminated";
                }
                this.error(msg);
                return false;
            }
            x = x * _base + d;
            Next();
            n--;
        }

        if (x > max || (0xd800 <= x && x < 0xe000))
        {
            this.error("escape sequence is invalid Unicode code point");
            return false;
        }

        return true;
    }
    private string scanString(char quote)
    {
        // '"' opening already consumed
        int offs = this.pos - 1;

        while (true)
        {
            var ch = this.ch;
            if (/*ch == "\n" ||*/ ch < '\0')
            {
                this.error("string literal not terminated");
                break;
            }
            Next();
            if (ch == quote)
            {
                break;
            }
            if (ch == '\\')
            {
                this.scanEscape(quote);
            }
        }
        //return this.input.substring(offs + 1, this.pos - 1);
        return input[(offs + 1)..(this.pos - 1)];
        //return this.input.substring(offs, this.pos);
    }
    public Item Scan()
    {

        char _ch;
        string lit = "*** ERROR ***";

        Token tok = Token.ILLEGAL;
        int offs = 0;

        bool markEOL = false;

        while (!this.eof)
        {
            this.SkipWhitespace();

            _ch = this.ch;
            offs = this.pos;
            //console.log(this.ch)

            markEOL = false;

            if (IsLetter(ch) || _ch == '_')
            {
                lit = this.scanIdentifier();
                //console.log(".....", lit)
                if (lit.Length > 1)
                {
                    tok = Keyword.GetType(lit);
                    switch (tok)
                    {
                        case Token.IDENT:
                        case Token.BREAK:
                        case Token.CONTINUE:
                        case Token.RETURN:
                            markEOL = true;
                            break;
                    }
                }
                else
                {
                    markEOL = true;
                    tok = Token.IDENT;
                }
                break;

            }
            else if (IsDecimal(ch) || ch == '.' && IsDecimal(this.peek()))
            {
                markEOL = true;

                (lit, tok) = this.ScanNumber();

                //console.log("this.scanNumber()", this.scanNumber())
                //{lit, tok} = this.scanNumber()
                //console.log(lit, tok)
                break;
            }
            else
            {

                Next();
                switch (ch)
                {
                    case '\0':
                        if (this.markEOL)
                        {
                            this.markEOL = false;
                            tok = Token.SEMICOLON;
                            lit = "EOF";

                        }
                        else
                        {
                            tok = Token.EOF;
                        }

                        break;

                    case '\n':
                        markEOL = false;
                        tok = Token.SEMICOLON;
                        lit = "\n";
                        break;
                    case '"':
                    case '\'':
                        /*if(this.useString){
                            tok = Token.STRING;
                            lit = this.scanString(ch);
                        }else{
                            tok = Token.SYMBOL;
                            lit = ch;
                        }*/
                        markEOL = true;
                        tok = Token.STRING;
                        lit = this.scanString(ch);
                        break;
                    case ':':
                        tok = this.evalOp(ch, Token.COLON, Token.LET, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '.':
                        tok = Token.DOT;
                        lit = ".";
                        break;
                    case '(':
                        tok = Token.LPAREN;
                        lit = "(";
                        break;
                    case ')':
                        markEOL = true;
                        tok = Token.RPAREN;
                        lit = ")";
                        break;
                    case '[':
                        tok = Token.LBRACK;
                        lit = "[";
                        break;

                    case ']':
                        markEOL = true;
                        tok = Token.RBRACK;
                        lit = "]";
                        break;
                    case '{':
                        tok = Token.LBRACE;
                        lit = "{";
                        break;
                    case '}':
                        //markEOL = true;
                        tok = Token.RBRACE;
                        lit = "}";
                        break;
                    case ',':
                        tok = Token.COMMA;
                        lit = ",";
                        break;
                    case ';':
                        tok = Token.SEMICOLON;
                        lit = ";";
                        break;
                    case '?':
                        tok = Token.QUESTION;
                        lit = "?";
                        if (this.ch == '?')
                        {
                            Next();
                            tok = Token.COALESCE;
                            lit = "??";
                        }
                        break;
                    case '+':
                        tok = this.evalOp(ch, Token.ADD, Token.ADD_ASSIGN, Token.INCR, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        if (tok == Token.INCR)
                        {
                            markEOL = true;
                        }
                        break;
                    case '-':
                        tok = this.evalOp(ch, Token.SUB, Token.SUB_ASSIGN, Token.DECR, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        if (tok == Token.DECR)
                        {
                            markEOL = true;
                        }
                        break;
                    case '*':
                        tok = this.evalOp(ch, Token.MUL, Token.MUL_ASSIGN, Token.POW, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '/':
                        tok = this.evalOp(ch, Token.DIV, Token.DIV_ASSIGN, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '%':
                        tok = this.evalOp(ch, Token.MOD, Token.MOD_ASSIGN, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '=':

                        tok = this.evalOp(ch, Token.ASSIGN, Token.EQL, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '!':

                        tok = this.evalOp(ch, Token.NOT, Token.NEQ, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '&':

                        tok = this.doubleOp(ch, Token.BIT_AND, Token.AND);
                        lit = this.input[offs..this.pos];

                        break;
                    case '|':

                        tok = this.doubleOp(ch, Token.BIT_OR, Token.OR);
                        lit = this.input[offs..this.pos];

                        break;
                    case '<':

                        tok = this.evalOp(ch, Token.LSS, Token.LEQ, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '>':

                        tok = this.evalOp(ch, Token.GTR, Token.GEQ, Token.ILLEGAL, Token.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '@':
                        tok = Token.AT;
                        lit = "@";
                        break;
                    case '$':
                        tok = Token.DOLAR;
                        lit = "$";
                        break;
                    case '#':
                        tok = Token.HASHTAG;
                        lit = "#";
                        break;
                    default:
                        markEOL = this.markEOL;
                        tok = Token.ILLEGAL;
                        lit = ch.ToString();
                        break;

                }
            }


            break;
        }
        //console.log(lit, tok)

        if (true)
        {
            this.markEOL = markEOL;
        }

        return new Item
        {
            Pos = this.pos,
            Value = lit,
            Priority = 0,
            Type = tok
        };




    }

    bool eof;

    public List<Item> getTokens()
    {
        List<Item> items = new();


        while (!this.eof)
        {
            items.Add(this.Scan());
        }

        items.Add(new Item
        {
            Pos = 1,
            Value = "EOF",
            Priority = 0,
            Type = Token.EOF
        });

        return items;
    }

    private void Next()
    {

        if (rd < input.Length)
        {
            pos = rd;
            ch = input[this.pos];
            rd++;
        }
        else
        {
            pos = input.Length;
            eof = true;
            ch = '\0';
        }

    }

    private void SetPosition(int position)
    {

        if (position < input.Length)
        {
            pos = position;
            ch = input[pos];
            rd = pos + 1;
        }
        else
        {
            pos = input.Length;
            eof = true;
            ch = '\0';
        }

    }

    private char peek()
    {
        if (pos < input.Length)
        {
            return input[pos];
        }
        return '\0';
    }
}