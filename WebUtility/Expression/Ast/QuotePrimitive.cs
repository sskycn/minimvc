using System.Text;
using Cvv.WebUtility.Mvc;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class QuotePrimitive: LiteralNode
    {
        public QuotePrimitive(Token relatedToken)
            : base(relatedToken)
        {

        }

        public override object Execute(IContext context, Stream writer)
        {
            return WebAppHelper.GetSkin(RelatedToken.Data).RenderView(context).TrimStart();
        }
    }
}
