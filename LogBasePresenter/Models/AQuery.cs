using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace LogBasePresenter.Models
{
    public enum EQueryType
    {
        CartQuery,
        GoodsCategoryQuery,
        GoodsQuery,
        MainPageQuery,
        PayQuery,
        SuccessPayQuery,
        UnparsedQuery
    };

    [Newtonsoft.Json.JsonConverter(typeof(JsonAQueryConverter))]
    public abstract class AQuery
    {
        public string QueryType
        {
            get {
                return Enum.Parse<EQueryType>(this.GetType().Name.ToString()).ToString();
            }
        }
        
        public string Url { get; set; }

        public static AQuery FromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments.Select(s => s.TrimEnd('/')).ToArray();
            if (segments.Length == 0)
            {
                throw new ArgumentException($"Invalid url: {url}");
            }
            if (segments.Length == 1)
            {
                return new MainPageQuery() { Url = url };
            }
            if (segments.Length == 2)
            {
                if (segments[1].Equals("cart", StringComparison.InvariantCulture))
                {
                    var parameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var cartQuery = new CartQuery
                    {
                        CartId = -1, GoodsAmount = -1, GoodsId = -1, Url = url
                    };
                    if (int.TryParse(parameters.Get("cart_id"), out var cartId))
                    {
                        cartQuery.CartId = cartId;
                    }
                    if (int.TryParse(parameters.Get("goods_id"), out var goodsId))
                    {
                        cartQuery.GoodsId = goodsId;
                    }
                    if (int.TryParse(parameters.Get("amount"), out var amount))
                    {
                        cartQuery.GoodsAmount = amount;
                    }
                    return cartQuery;
                }
                else if(segments[1].Equals("pay", StringComparison.InvariantCulture))
                {
                    var parameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var payQuery = new PayQuery
                    {
                        CartId = -1,
                        UserId = -1,
                        Url = url
                    };
                    if (int.TryParse(parameters.Get("cart_id"), out var cartId))
                    {
                        payQuery.CartId = cartId;
                    }
                    if (long.TryParse(parameters.Get("user_id"), out var userId))
                    {
                        payQuery.UserId = userId;
                    }
                    return payQuery;
                } 
                else
                {
                    var regex = new Regex(@"success_pay_(\d+)/?");
                    var result = regex.Match(segments[1]);
                    if (result.Success)
                    {
                        return new SuccessPayQuery {Url = url, TransactionId = int.Parse(result.Groups[1].Value)};
                    }
                    else
                    {
                        return new GoodsCategoryQuery() {Url = url, GoodsCategoryName = segments[1]};
                    }
                }
            }
            if (segments.Length == 3)
            {
                return new GoodsQuery() { Url = url, GoodsCategoryName = segments[1], GoodsName = segments[2] };
            }

            return new UnparsedQuery(url);
        }
    }

    public class JsonAQueryResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(AQuery).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    class JsonAQueryConverter : JsonConverter
    {
        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new JsonAQueryResolver() };

        public override bool CanConvert(Type objectType) => objectType == typeof(AQuery);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["Url"] != null)
            {
                return AQuery.FromUrl(jo["Url"].Value<string>());
            }
            throw new JsonReaderException();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new JsonSerializationException();
        }
    }
}
