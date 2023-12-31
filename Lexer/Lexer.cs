namespace Whendy;

public struct Token
{
    public Token(int pos, string value, int priority, TokenType type)
    {
        this.Pos = pos;
        this.Value = value;
        this.Priority = priority;
        this.Type = type;

    }
    public int Pos;
    public string Value;
    public int Priority;
    public TokenType Type;



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

    private TokenType evalOp(char ch, TokenType tokenDefault, TokenType tokenAssign, TokenType tokenX2, TokenType tokenX3)
    {
        if (this.ch == '=')
        {
            Next();
            return tokenAssign;
        }
        if (tokenX2 != TokenType.ILLEGAL && this.ch == ch)
        {
            Next();
            if (tokenX3 != TokenType.ILLEGAL && this.ch == ch)
            {
                Next();
                return tokenX3;
            }
            return tokenX2;
        }
        return tokenDefault;
    }

    private TokenType doubleOp(char ch, TokenType tokenDefault, TokenType tokenX2)
    {
        if (tokenX2 != TokenType.ILLEGAL && this.ch == ch)
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
    private (string, TokenType) ScanNumber()
    {
        int offs = this.pos;
        TokenType tok = TokenType.INT;

        int _base = 10;        // number base
        char prefix = '0'; // one of 0 (decimal), '0' (0-octal), 'x', 'o', or 'b'
        //digsep = 0       // bit 0: digit present, bit 1: '_' present
        //invalid = -1     // index of invalid digit in literal, or < 0

        string lit = "c";
        // integer part
        if (this.ch != '.')
        {
            tok = TokenType.INT;
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
            tok = TokenType.FLOAT;
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
            tok = TokenType.FLOAT;
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

    string stripCR(string b, bool comment)
    {
        string c = "";
        int i = 0;
        int j = 0;
        foreach (char ch in b)
        {
            if (ch != '\r' || comment && i > "/*".Length && c[i - 1] == '*' && j + 1 < b.Length && b[j + 1] == '/')
            {
                c += ch;
                i++;
            }
            j++;
        }
        return c;
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

    private string scanRawString()
    {
        // '`' opening already consumed
        var offs = rd - 1;

        bool hasCR = false;
        while (true)
        {
            var ch = this.ch;
            if (ch < 0)
            {
                this.error("raw string literal not terminated");
                break;
            }
            Next();
            if (ch == '`')
            {
                break;
            }
            if (ch == '\r')
            {
                hasCR = true;
            }
        }

        var lit = input[offs..this.rd];
        if (hasCR)
        {
            lit = stripCR(lit, false);
        }

        return lit;
    }

    public Token Scan()
    {

        char ch;
        string lit = "*** ERROR ***";

        TokenType tok = TokenType.ILLEGAL;
        int offs = 0;

        bool markEOL = false;

        while (!this.eof)
        {
            this.SkipWhitespace();

            ch = this.ch;
            offs = this.pos;
            //console.log(this.ch)

            markEOL = false;

            if (IsLetter(ch) || ch == '_')
            {
                lit = this.scanIdentifier();
                //console.log(".....", lit)
                if (lit.Length > 1)
                {
                    tok = Keyword.GetType(lit);
                    switch (tok)
                    {
                        case TokenType.IDENT:
                        case TokenType.BREAK:
                        case TokenType.CONTINUE:
                        case TokenType.RETURN:
                            markEOL = true;
                            break;
                    }
                }
                else
                {
                    markEOL = true;
                    tok = TokenType.IDENT;
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
                            tok = TokenType.SEMICOLON;
                            lit = "EOF";

                        }
                        else
                        {
                            tok = TokenType.EOF;
                        }

                        break;

                    case '\n':
                        markEOL = false;
                        tok = TokenType.SEMICOLON;
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
                        tok = TokenType.STRING;
                        lit = this.scanString(ch);
                        break;
                    case ':':
                        tok = this.evalOp(ch, TokenType.COLON, TokenType.LET, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '.':
                        tok = TokenType.DOT;
                        lit = ".";
                        break;
                    case '(':
                        tok = TokenType.LPAREN;
                        lit = "(";
                        break;
                    case ')':
                        markEOL = true;
                        tok = TokenType.RPAREN;
                        lit = ")";
                        break;
                    case '[':
                        tok = TokenType.LBRACK;
                        lit = "[";
                        break;

                    case ']':
                        markEOL = true;
                        tok = TokenType.RBRACK;
                        lit = "]";
                        break;
                    case '{':
                        tok = TokenType.LBRACE;
                        lit = "{";
                        break;
                    case '}':
                        //markEOL = true;
                        tok = TokenType.RBRACE;
                        lit = "}";
                        break;
                    case ',':
                        tok = TokenType.COMMA;
                        lit = ",";
                        break;
                    case ';':
                        tok = TokenType.SEMICOLON;
                        lit = ";";
                        break;
                    case '?':
                        tok = TokenType.QUESTION;
                        lit = "?";
                        if (this.ch == '?')
                        {
                            Next();
                            tok = TokenType.COALESCE;
                            lit = "??";
                        }
                        break;
                    case '+':
                        tok = this.evalOp(ch, TokenType.ADD, TokenType.ADD_ASSIGN, TokenType.INCR, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        if (tok == TokenType.INCR)
                        {
                            markEOL = true;
                        }
                        break;
                    case '-':
                        tok = this.evalOp(ch, TokenType.SUB, TokenType.SUB_ASSIGN, TokenType.DECR, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        if (tok == TokenType.DECR)
                        {
                            markEOL = true;
                        }
                        break;
                    case '*':
                        tok = this.evalOp(ch, TokenType.MUL, TokenType.MUL_ASSIGN, TokenType.POW, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '/':
                        tok = this.evalOp(ch, TokenType.DIV, TokenType.DIV_ASSIGN, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '%':
                        tok = this.evalOp(ch, TokenType.MOD, TokenType.MOD_ASSIGN, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];
                        break;
                    case '=':

                        tok = this.evalOp(ch, TokenType.ASSIGN, TokenType.EQL, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '!':

                        tok = this.evalOp(ch, TokenType.NOT, TokenType.NEQ, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '&':

                        tok = this.doubleOp(ch, TokenType.BIT_AND, TokenType.AND);
                        lit = this.input[offs..this.pos];

                        break;
                    case '|':

                        tok = this.doubleOp(ch, TokenType.BIT_OR, TokenType.OR);
                        lit = this.input[offs..this.pos];

                        break;
                    case '<':

                        tok = this.evalOp(ch, TokenType.LSS, TokenType.LEQ, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '>':

                        tok = this.evalOp(ch, TokenType.GTR, TokenType.GEQ, TokenType.ILLEGAL, TokenType.ILLEGAL);
                        lit = this.input[offs..this.pos];

                        break;
                    case '@':
                        tok = TokenType.AT;
                        lit = "@";
                        break;
                    case '$':
                        tok = TokenType.DOLAR;
                        lit = "$";
                        break;
                    case '#':
                        tok = TokenType.HASHTAG;
                        lit = "#";
                        break;
                    default:
                        markEOL = this.markEOL;
                        tok = TokenType.ILLEGAL;
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

        return new Token
        {
            Pos = this.pos,
            Value = lit,
            Priority = 0,
            Type = tok
        };




    }

    bool eof;

    public List<Token> getTokens()
    {
        List<Token> Tokens = new();


        while (!this.eof)
        {
            Tokens.Add(this.Scan());
        }

        Tokens.Add(new Token
        {
            Pos = 1,
            Value = "EOF",
            Priority = 0,
            Type = TokenType.EOF
        });

        return Tokens;
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