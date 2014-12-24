
namespace Cvv.WebUtility.Expression
{
    public enum TokenId
    {
        Html,           	//(.)+
        Newline,        	//\r|\n
        Comment,        	//	//
        Ident,              // [_a-zA-Z][_a-zA-Z0-9]*
        Quote,              // ~/
        Unknow,             //

        Dollar,             //$
        Block,              //${

        LParen,         	//(
        RParen,         	//)
        LCurly,         	//{
        RCurly,         	//}
        LBracket,       	//[
        RBracket,       	//]

        True,           	//true
        False,          	//false
        Null,           	//null
        Int,            	//int
        Double,         	//double
        String,         	//".+"

        Var,            	//var
        For,        	    //for
        In,                 //in
        If,             	//if
        Else,           	//else
        Continue,       	//continue
        Break,          	//break
        Echo,           	//echo

        Plus,           	//+
        Minus,          	//-
        Star,           	//*
        Slash,          	///
        Percent,        	//%
        PlusPlus,       	//++
        MinusMinus,     	//--
        PlusEqual,      	//+=
        MinusEqual,     	//-=
        StarEqual,      	//*=
        SlashEqual,     	///=
        PercentEqual,   	//%=

        And,            	//&&
        Or,             	//||
        Not,            	//!

        //BinaryAnd,          //&
        //BinaryOr,           //|
        //BinaryNot,          //!

        Equal,          	//=

        EqualEqual,     	//==
        NotEqual,       	//!=
        GreaterEqual,   	//>=
        LessEqual,      	//<=
        Greater,        	//>
        Less,           	//<

        Comma,          	//,
        Dot,            	//.
        Semi,           	//;

        Question,       	//?
        QuestionQuestion,	//??
        Colon,          	//:
        Eof,                //\0
    }
}
