using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTYD.Model
{
    // Model for Upcoming Menu https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu 
    public partial class UpcomingMenu
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        //added 12/28
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("result")]
        public Result[] Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("menu_uid")]
        public string MenuUid { get; set; }

        [JsonProperty("menu_date")]
        public string MenuDate { get; set; }

        [JsonProperty("menu_category")]
        public string MenuCategory { get; set; }

        [JsonProperty("menu_type")]
        public string MenuType { get; set; }

        [JsonProperty("meal_cat")]
        public string MealCat { get; set; }

        [JsonProperty("menu_meal_id")]
        public string MenuMealId { get; set; }

        [JsonProperty("default_meal")]
        public string DefaultMeal { get; set; }

        [JsonProperty("delivery_days")]
        public string DeliveryDays { get; set; }

        [JsonProperty("menu_meal_price")]
        public long MenuMealPrice { get; set; }

        //public string meal_uid = "0";
        //public string meal_category = "none";
        //public string meal_name = "beans";
        //public string meal_desc = "no";
        //public string meal_hint = "s";
        //public string meal_photo_URL = "http";
        [JsonProperty("meal_uid")]
        public string MealUid { get; set; }

        [JsonProperty("meal_business")]
        public string MealBusiness { get; set; }

        [JsonProperty("meal_category")]
        public string MealCategory { get; set; }

        [JsonProperty("meal_name")]
        public string MealName { get; set; }

        [JsonProperty("meal_desc")]
        public string MealDesc { get; set; }

        [JsonProperty("meal_hint")]
        public string MealHint { get; set; }

        [JsonProperty("meal_photo_URL")]
        public string MealPhotoUrl { get; set; }

        [JsonProperty("meal_calories")]
        public long MealCalories { get; set; }

        [JsonProperty("meal_protein")]
        public double MealProtein { get; set; }

        [JsonProperty("meal_carbs")]
        public double MealCarbs { get; set; }

        [JsonProperty("meal_fiber")]
        public double MealFiber { get; set; }

        [JsonProperty("meal_sugar")]
        public double MealSugar { get; set; }

        [JsonProperty("meal_fat")]
        public double MealFat { get; set; }

        [JsonProperty("meal_sat")]
        public double MealSat { get; set; }

        [JsonProperty("meal_cost")]
        public string MealCost { get; set; }

        [JsonProperty("meal_price")]
        public string MealPrice { get; set; }

        [JsonProperty("meal_price_addon")]
        public string MealPriceAddon { get; set; }

        [JsonProperty("meal_status")]
        public string MealStatus { get; set; }
    }

    public enum DefaultMeal { False, True };

    public enum DeliveryDays { SundayMonday };

    public enum MealCat { AddOn, Breakfast, Entree, Salad, Smoothie, Soup };

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MenuCategory { AddOn1, AddOn2, AddOn3, AddOn4, SeasFave1, SeasFave2, SeasFave3, Smoothie1, Smoothie2, Smoothie3, WklySpcl1, WklySpcl2, WklySpcl3 };

    public enum MenuType { Breakfast, LocalTreat, SeasonalEntree, SeasonalSalad, Smoothie, WeeklyEntree, WeeklySalad, WeeklySoup };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DefaultMealConverter.Singleton,
                DeliveryDaysConverter.Singleton,
                MealCatConverter.Singleton,
                MenuCategoryConverter.Singleton,
                MenuTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DefaultMealConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DefaultMeal) || t == typeof(DefaultMeal?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "FALSE":
                    return DefaultMeal.False;
                case "TRUE":
                    return DefaultMeal.True;
            }
            throw new Exception("Cannot unmarshal type DefaultMeal");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DefaultMeal)untypedValue;
            switch (value)
            {
                case DefaultMeal.False:
                    serializer.Serialize(writer, "FALSE");
                    return;
                case DefaultMeal.True:
                    serializer.Serialize(writer, "TRUE");
                    return;
            }
            throw new Exception("Cannot marshal type DefaultMeal");
        }

        public static readonly DefaultMealConverter Singleton = new DefaultMealConverter();
    }

    internal class DeliveryDaysConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DeliveryDays) || t == typeof(DeliveryDays?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "[Sunday, Monday]")
            {
                return DeliveryDays.SundayMonday;
            }
            throw new Exception("Cannot unmarshal type DeliveryDays");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DeliveryDays)untypedValue;
            if (value == DeliveryDays.SundayMonday)
            {
                serializer.Serialize(writer, "[Sunday, Monday]");
                return;
            }
            throw new Exception("Cannot marshal type DeliveryDays");
        }

        public static readonly DeliveryDaysConverter Singleton = new DeliveryDaysConverter();
    }

    internal class MealCatConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MealCat) || t == typeof(MealCat?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Add-On":
                    return MealCat.AddOn;
                case "Breakfast":
                    return MealCat.Breakfast;
                case "Entree":
                    return MealCat.Entree;
                case "Salad":
                    return MealCat.Salad;
                case "Smoothie":
                    return MealCat.Smoothie;
                case "Soup":
                    return MealCat.Soup;
            }
            throw new Exception("Cannot unmarshal type MealCat");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MealCat)untypedValue;
            switch (value)
            {
                case MealCat.AddOn:
                    serializer.Serialize(writer, "Add-On");
                    return;
                case MealCat.Breakfast:
                    serializer.Serialize(writer, "Breakfast");
                    return;
                case MealCat.Entree:
                    serializer.Serialize(writer, "Entree");
                    return;
                case MealCat.Salad:
                    serializer.Serialize(writer, "Salad");
                    return;
                case MealCat.Smoothie:
                    serializer.Serialize(writer, "Smoothie");
                    return;
                case MealCat.Soup:
                    serializer.Serialize(writer, "Soup");
                    return;
            }
            throw new Exception("Cannot marshal type MealCat");
        }

        public static readonly MealCatConverter Singleton = new MealCatConverter();
    }

    internal class MenuCategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MenuCategory) || t == typeof(MenuCategory?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ADD_ON_1":
                    return MenuCategory.AddOn1;
                case "ADD_ON_2":
                    return MenuCategory.AddOn2;
                case "ADD_ON_3":
                    return MenuCategory.AddOn3;
                case "ADD_ON_4":
                    return MenuCategory.AddOn4;
                case "SEAS_FAVE_1":
                    return MenuCategory.SeasFave1;
                case "SEAS_FAVE_2":
                    return MenuCategory.SeasFave2;
                case "SEAS_FAVE_3":
                    return MenuCategory.SeasFave3;
                case "SMOOTHIE_1":
                    return MenuCategory.Smoothie1;
                case "SMOOTHIE_2":
                    return MenuCategory.Smoothie2;
                case "SMOOTHIE_3":
                    return MenuCategory.Smoothie3;
                case "WKLY_SPCL_1":
                    return MenuCategory.WklySpcl1;
                case "WKLY_SPCL_2":
                    return MenuCategory.WklySpcl2;
                case "WKLY_SPCL_3":
                    return MenuCategory.WklySpcl3;
            }
            throw new Exception("Cannot unmarshal type MenuCategory");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MenuCategory)untypedValue;
            switch (value)
            {
                case MenuCategory.AddOn1:
                    serializer.Serialize(writer, "ADD_ON_1");
                    return;
                case MenuCategory.AddOn2:
                    serializer.Serialize(writer, "ADD_ON_2");
                    return;
                case MenuCategory.AddOn3:
                    serializer.Serialize(writer, "ADD_ON_3");
                    return;
                case MenuCategory.AddOn4:
                    serializer.Serialize(writer, "ADD_ON_4");
                    return;
                case MenuCategory.SeasFave1:
                    serializer.Serialize(writer, "SEAS_FAVE_1");
                    return;
                case MenuCategory.SeasFave2:
                    serializer.Serialize(writer, "SEAS_FAVE_2");
                    return;
                case MenuCategory.SeasFave3:
                    serializer.Serialize(writer, "SEAS_FAVE_3");
                    return;
                case MenuCategory.Smoothie1:
                    serializer.Serialize(writer, "SMOOTHIE_1");
                    return;
                case MenuCategory.Smoothie2:
                    serializer.Serialize(writer, "SMOOTHIE_2");
                    return;
                case MenuCategory.Smoothie3:
                    serializer.Serialize(writer, "SMOOTHIE_3");
                    return;
                case MenuCategory.WklySpcl1:
                    serializer.Serialize(writer, "WKLY_SPCL_1");
                    return;
                case MenuCategory.WklySpcl2:
                    serializer.Serialize(writer, "WKLY_SPCL_2");
                    return;
                case MenuCategory.WklySpcl3:
                    serializer.Serialize(writer, "WKLY_SPCL_3");
                    return;
            }
            throw new Exception("Cannot marshal type MenuCategory");
        }

        public static readonly MenuCategoryConverter Singleton = new MenuCategoryConverter();
    }

    internal class MenuTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(MenuType) || t == typeof(MenuType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Breakfast":
                    return MenuType.Breakfast;
                case "Local Treat":
                    return MenuType.LocalTreat;
                case "Seasonal Entree":
                    return MenuType.SeasonalEntree;
                case "Seasonal Salad":
                    return MenuType.SeasonalSalad;
                case "Smoothie":
                    return MenuType.Smoothie;
                case "Weekly Entree":
                    return MenuType.WeeklyEntree;
                case "Weekly Salad":
                    return MenuType.WeeklySalad;
                case "Weekly Soup":
                    return MenuType.WeeklySoup;
            }
            throw new Exception("Cannot unmarshal type MenuType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (MenuType)untypedValue;
            switch (value)
            {
                case MenuType.Breakfast:
                    serializer.Serialize(writer, "Breakfast");
                    return;
                case MenuType.LocalTreat:
                    serializer.Serialize(writer, "Local Treat");
                    return;
                case MenuType.SeasonalEntree:
                    serializer.Serialize(writer, "Seasonal Entree");
                    return;
                case MenuType.SeasonalSalad:
                    serializer.Serialize(writer, "Seasonal Salad");
                    return;
                case MenuType.Smoothie:
                    serializer.Serialize(writer, "Smoothie");
                    return;
                case MenuType.WeeklyEntree:
                    serializer.Serialize(writer, "Weekly Entree");
                    return;
                case MenuType.WeeklySalad:
                    serializer.Serialize(writer, "Weekly Salad");
                    return;
                case MenuType.WeeklySoup:
                    serializer.Serialize(writer, "Weekly Soup");
                    return;
            }
            throw new Exception("Cannot marshal type MenuType");
        }

        public static readonly MenuTypeConverter Singleton = new MenuTypeConverter();
    }
}

