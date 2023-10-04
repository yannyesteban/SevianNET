using System.Diagnostics.Tracing;



namespace Whendy;



public enum Token
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
    private readonly static Dictionary<string, Whendy.Token> keywords = new(){
        {"if", Token.IF},
        {"else", Token.ELSE},
        {"case", Token.CASE},
        {"when", Token.WHEN},
        {"while", Token.WHILE},
        {"do", Token.DO},
        {"default", Token.DEFAULT},
        {"for", Token.FOR},
        {"each", Token.EACH},
        {"false", Token.FALSE},
        {"true", Token.TRUE},
        {"null", Token.NULL},
        {"return", Token.RETURN},
        {"break", Token.BREAK},
        {"continue", Token.CONTINUE},
        {"let", Token.CONTINUE},
        {"class", Token.CLASS},
        {"func", Token.FUNC},
        {"fn", Token.FN},
        {"print", Token.PRINT},
    };


    public static Token GetType(String token)
    {


        //Keyword.keywords.Add("if", Whendy.TokenType.IF);
        if (keywords.ContainsKey(token))
        {
            return keywords[token];
        }

        return Token.IDENT;
    }

    public static int precedence(Token op) {
        return op switch
        {
            Token.OR => 1,
            Token.AND => 2,
            Token.EQL or Token.NEQ or Token.LSS or Token.LEQ or Token.GTR or Token.GEQ => 3,
            Token.ADD or Token.SUB => 4,//case Token.OR:
            Token.MUL or Token.DIV or Token.MOD => 5,//case Token.AND:
            _ => LowestPrec,
        };
    }
}
