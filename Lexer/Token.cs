using System.Diagnostics.Tracing;



namespace Whendy;



public enum TokenType
{
    ILLEGAL,
    IDENT = 1, // main
    EOF,
    EOL, // \n
    COMMENT,
    INT, // 12345
    FLOAT, // 123.45
           //IMAG,   // 123.45i
           //CHAR,   // 'a'
    STRING, // "abc"
    ADD, // +
    SUB, // -
    MUL, // *
    DIV, // /
    MOD, // %
    BIT_AND,     // &
    BIT_OR,      // |
    BIT_XOR,     // ^
    BIT_SHL,     // <<
    BIT_SHR,     // >>
    BIT_AND_NOT, // &^
    AT, //@
    DOLAR, //$
    HASHTAG, //#
    QUESTION, // ? 
    COALESCE, // ?? 
    LPAREN, // (
    LBRACK, // [
    LBRACE, // {
    COMMA, // ,
    DOT, // .
    RPAREN, // )
    RBRACK, // ]
    RBRACE, // }
    SEMICOLON, // ;
    COLON, // :
    ADD_ASSIGN, // +=
    SUB_ASSIGN, // -=
    MUL_ASSIGN, // *=
    DIV_ASSIGN, // /=
    MOD_ASSIGN, // %=
    AND, // &&
    OR, // ||
    INCR, // ++
    DECR, // --
    POW, // **
    EQL, // ==
    LSS, // <
    GTR, // >
    ASSIGN, // =
    NOT, // (!)
    NEQ, // (!=)
    LEQ, // <=
    GEQ, // >=
    LET, // :=
    SYMBOL,
    IF, // "if"
    ELSE, // "else"
    CASE, // "case"
    WHEN, // "when"
    WHILE, // "while"
    DO, // "DO"
    DEFAULT, // "default"
    FOR, // "for"
    EACH, // "each"
    FALSE, // "false"
    TRUE, // "true"
    NULL, // "true"
    RETURN, // "return"
    BREAK, // "break"
    CONTINUE, // "continue"
    CLASS, // "class"
    FUNC, // "func"
    FN, // "fn"
    PRINT, // "print"
    LEFT_DELIM,
    RIGHT_DELIM
}



public static class Keyword
{
    private readonly static int LowestPrec = 0;
    private readonly static Dictionary<string, Whendy.TokenType> keywords = new(){
        {"if", TokenType.IF},
        {"else", TokenType.ELSE},
        {"case", TokenType.CASE},
        {"when", TokenType.WHEN},
        {"while", TokenType.WHILE},
        {"do", TokenType.DO},
        {"default", TokenType.DEFAULT},
        {"for", TokenType.FOR},
        {"each", TokenType.EACH},
        {"false", TokenType.FALSE},
        {"true", TokenType.TRUE},
        {"null", TokenType.NULL},
        {"return", TokenType.RETURN},
        {"break", TokenType.BREAK},
        {"continue", TokenType.CONTINUE},
        {"let", TokenType.CONTINUE},
        {"class", TokenType.CLASS},
        {"func", TokenType.FUNC},
        {"fn", TokenType.FN},
        {"print", TokenType.PRINT},
    };


    public static TokenType GetType(String token)
    {


        //Keyword.keywords.Add("if", Whendy.TokenType.IF);
        if (keywords.ContainsKey(token))
        {
            return keywords[token];
        }

        return TokenType.IDENT;
    }

    public static int precedence(TokenType op) {
        return op switch
        {
            TokenType.OR => 1,
            TokenType.AND => 2,
            TokenType.EQL or TokenType.NEQ or TokenType.LSS or TokenType.LEQ or TokenType.GTR or TokenType.GEQ => 3,
            TokenType.ADD or TokenType.SUB => 4,//case Token.OR:
            TokenType.MUL or TokenType.DIV or TokenType.MOD => 5,//case Token.AND:
            _ => LowestPrec,
        };
    }
}
