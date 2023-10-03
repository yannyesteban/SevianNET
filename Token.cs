using System.Diagnostics.Tracing;



namespace Whendy;



    public enum Token
    {
        IDENT = 1, // main
        ILLEGAL,
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
        RIGHT_DELIM,

    }


    
    public static class Keyword
    {
         public static Dictionary<string, Whendy.Token> keywords = new Dictionary<string, Whendy.Token>();

        
        public static Token getType(String token)
        {

            
            if(keywords.ContainsKey(token)){
                return keywords[token];
            }
            
            return Token.IDENT;
        }
    }

Keyword.keywords.Add("if", Whendy.Token.IF);