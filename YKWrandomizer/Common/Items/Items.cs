﻿using System;
using YKWrandomizer.Logic;
using System.Collections.Generic;

namespace YKWrandomizer.Common
{
    public static class Items
    {
        public static IDictionary<UInt32, Item> YW1 = new Dictionary<UInt32, Item>
        {
            {0x26AA1145, new Item("Worn Bangle")},
            {0x9CFB18DC, new Item("Rocker Wrist")},
            {0x0ACB1FAB, new Item("Brute Bracer")},
            {0xA95E7B35, new Item("Sun Bracelet")},
            {0x3F6E7C42, new Item("Fiend Band")},
            {0x11C0D344, new Item("Rusty Ring")},
            {0xAB91DADD, new Item("Pretty Ring")},
            {0x3DA1DDAA, new Item("Illusion Ring")},
            {0x9E34B934, new Item("Lunar Ring")},
            {0x0804BE43, new Item("Fiend Ring")},
            {0xB255B7DA, new Item("Fire Ring")},
            {0x2465B0AD, new Item("Water Ring")},
            {0xB5780F3D, new Item("Lightning Ring")},
            {0x2348084A, new Item("Earth Ring")},
            {0xC6C1CF2A, new Item("Ice Ring")},
            {0x50F1C85D, new Item("Wind Ring")},
            {0x487E9546, new Item("Aged Charm")},
            {0xF22F9CDF, new Item("Runic Charm")},
            {0x641F9BA8, new Item("Armor Charm")},
            {0xC78AFF36, new Item("Galaxy Charm")},
            {0x51BAF841, new Item("Fiend Charm")},
            {0xEBEBF1D8, new Item("Blaze Charm")},
            {0x7DDBF6AF, new Item("Flood Charm")},
            {0xECC6493F, new Item("Bolt Charm")},
            {0x7AF64E48, new Item("Quake Charm")},
            {0x9F7F8928, new Item("Frost Charm")},
            {0x094F8E5F, new Item("Storm Charm")},
            {0x7F145747, new Item("Simple Badge")},
            {0xC5455EDE, new Item("Shiny Badge")},
            {0x537559A9, new Item("Hermes Badge")},
            {0xF0E03D37, new Item("Meteor Badge")},
            {0x66D03A40, new Item("Fiend Badge")},
            {0xFA021842, new Item("Cicada Sword")},
            {0x405311DB, new Item("Beefy Bell")},
            {0xD66316AC, new Item("Spell Bell")},
            {0x75F67232, new Item("Tough Bell")},
            {0xE3C67545, new Item("Speed Bell")},
            {0x59977CDC, new Item("Big Bottle")},
            {0xCFA77BAB, new Item("Tengu Fan")},
            {0x5EBAC43B, new Item("Cheery Coat")},
            {0xC88AC34C, new Item("Nail Bat")},
            {0x2D03042C, new Item("Reversword")},
            {0xBB33035B, new Item("Turnabeads")},
            {0x01620AC2, new Item("Reflector")},
            {0xCD68DA43, new Item("Ritzy Studs")},
            {0x7739D3DA, new Item("Sleep 'n' Study")},
            {0xE109D4AD, new Item("Die of Fate")},
            {0x429CB033, new Item("Iron Plates")},
            {0xD4ACB744, new Item("Thick Specs")},
            {0x6EFDBEDD, new Item("Ancient Scale")},
            {0xF8CDB9AA, new Item("Venoct Gauntlet")},
            {0x69D0063A, new Item("Heavenly Sash")},
            {0xFFE0014D, new Item("Ski Mask")},
            {0x1A69C62D, new Item("Sticker of Hate")},
            {0x8C59C15A, new Item("Vampiric Fangs")},
            {0x3608C8C3, new Item("Crystal Ball")},
            {0xA038CFB4, new Item("Sleepillow")},
            {0x94D69C41, new Item("Restraint Belt")},
            {0x2E8795D8, new Item("Guard Gem")},
            {0xB8B792AF, new Item("Monkey Circlet")},
            {0x7AC0856B, new Item("Plum Rice Ball")},
            {0xC0918CF2, new Item("Leaf Rice Ball")},
            {0x56A18B85, new Item("Roe Rice Ball")},
            {0xF534EF1B, new Item("Shrimp Rice Ball")},
            {0x4DAA476A, new Item("Sandwich")},
            {0xF7FB4EF3, new Item("Custard Bread")},
            {0x546E2A6D, new Item("Curry Bread")},
            {0x61CB4984, new Item("Baguette")},
            {0xC25E2D1A, new Item("Blehgel")},
            {0x14140168, new Item("10p Gum")},
            {0xAE4508F1, new Item("Gooey Candy")},
            {0x38750F86, new Item("Giant Cracker")},
            {0x9BE06B18, new Item("Fruit Drops")},
            {0x0DD06C6F, new Item("Shaved Ice")},
            {0xB78165F6, new Item("Candy Apple")},
            {0x237EC369, new Item("Milk")},
            {0x992FCAF0, new Item("Coffee Milk")},
            {0x0F1FCD87, new Item("Fruit Milk")},
            {0xAC8AA919, new Item("Amazing Milk")},
            {0xA6688C6C, new Item("Y-Cola")},
            {0x1C3985F5, new Item("Soul Tea")},
            {0x8A098282, new Item("Spiritizer Y")},
            {0x299CE61C, new Item("VoltXtreme")},
            {0x91024E6D, new Item("Hamburger")},
            {0x2B5347F4, new Item("Cheeseburger")},
            {0xBD634083, new Item("Double Burger")},
            {0x1EF6241D, new Item("Nom Burger")},
            {0xFFD6CA6E, new Item("Ramen Cup")},
            {0x4587C3F7, new Item("Pork Ramen")},
            {0xD3B7C480, new Item("Deluxe Ramen")},
            {0x7022A01E, new Item("Everything Ramen")},
            {0xF5FB5464, new Item("Pot Stickers")},
            {0x4FAA5DFD, new Item("Liver &amp; Chives")},
            {0xD99A5A8A, new Item("Crab Omelet")},
            {0x7A0F3E14, new Item("Chili Shrimp")},
            {0x28CDFBD2, new Item("Carrot")},
            {0x929CF24B, new Item("Cucumber")},
            {0x04ACF53C, new Item("Bamboo Shoot")},
            {0xA73991A2, new Item("Matsutake")},
            {0x7173BDD0, new Item("Chicken Thigh")},
            {0xCB22B449, new Item("Slab Bacon")},
            {0x5D12B33E, new Item("Beef Tongue")},
            {0xFE87D7A0, new Item("Marbled Beef")},
            {0x46197FD1, new Item("Dried Mackerel")},
            {0xFC487648, new Item("Yellowtail")},
            {0x6A78713F, new Item("Fresh Urchin")},
            {0xC9ED15A1, new Item("Choice Tuna")},
            {0xC0EA2013, new Item("Small Exporb")},
            {0x7ABB298A, new Item("Medium Exporb")},
            {0xEC8B2EFD, new Item("Large Exporb")},
            {0x4F1E4A63, new Item("Mega Exporb")},
            {0xD92E4D14, new Item("Holy Exporb")},
            {0x7296AD17, new Item("Staminum")},
            {0xC8C7A48E, new Item("Staminum Alpha")},
            {0xAE3EA410, new Item("Hidden Hits")},
            {0x146FAD89, new Item("Top Techniques")},
            {0x825FAAFE, new Item("Soul Secrets")},
            {0x9379F81B, new Item("A Serious Life")},
            {0x2928F182, new Item("Think Karate")},
            {0xBF18F6F5, new Item("Use Karate")},
            {0x1C8D926B, new Item("Skill Compendium")},
            {0x8ABD951C, new Item("Skill Encycloped.")},
            {0x30EC9C85, new Item("Get Guarding")},
            {0xA6DC9BF2, new Item("Guard Gloriously")},
            {0x37C12462, new Item("Li'l Angel Heals")},
            {0xA1F12315, new Item("Bye, Li'l Angel")},
            {0x4478E475, new Item("The Pest's Quest")},
            {0xD248E302, new Item("The Perfect Pest")},
            {0x6819EA9B, new Item("Support Life #7")},
            {0xFE29EDEC, new Item("Support Special")},
            {0x2B28EB15, new Item("Strength Talisman")},
            {0x9179E28C, new Item("Spirit Talisman")},
            {0x0749E5FB, new Item("Defense Talisman")},
            {0xA4DC8165, new Item("Speed Talisman")},
            {0x1C422914, new Item("Nasty Medicine")},
            {0xA613208D, new Item("Bitter Medicine")},
            {0x302327FA, new Item("Mighty Medicine")},
            {0x45FC6F16, new Item("Getaway Plush")},
            {0x99546611, new Item("Bronze Doll")},
            {0x23056F88, new Item("Silver Doll")},
            {0xB53568FF, new Item("Golden Doll")},
            {0x4E4F57AD, new Item("Fish Bait")},
            {0xF41E5E34, new Item("Black Syrup")},
            {0x7CBF899A, new Item("Red Coin")},
            {0xC6EE8003, new Item("Yellow Coin")},
            {0x50DE8774, new Item("Orange Coin")},
            {0xF34BE3EA, new Item("Pink Coin")},
            {0x657BE49D, new Item("Green Coin")},
            {0xDF2AED04, new Item("Blue Coin")},
            {0x491AEA73, new Item("Purple Coin")},
            {0xD80755E3, new Item("Light-Blue Coin")},
            {0xA3893CBB, new Item("Sapphire Coin")},
            {0x35B93BCC, new Item("Emerald Coin")},
            {0x962C5F52, new Item("Ruby Coin")},
            {0x001C5825, new Item("Topaz Coin")},
            {0x19D83522, new Item("Diamond Coin")},
            {0xBA4D51BC, new Item("Excitement Coin")},
            {0x2C7D56CB, new Item("Five-Star Coin")},
            {0xBD60E95B, new Item("Special Coin")},
            {0x792595AC, new Item("Dancing Star")},
            {0xA4133A1A, new Item("Legendary Blade")},
            {0x1E423383, new Item("Cursed Blade")},
            {0x07865E84, new Item("Holy Blade")},
            {0x887234F4, new Item("General's Soul")},
            {0x2BE7506A, new Item("Love Buster")},
            {0xBDD7571D, new Item("GHz Orb")},
            {0x00ABE663, new Item("Unbeatable Soul")},
            {0x5F73289A, new Item("Platinum Bar")},
            {0x969BE114, new Item("Snowstorm Cloak")},
            {0x73122674, new Item("Love Scepter")},
            {0x91B659F3, new Item("Glacial Clip")},
            {0xE5222103, new Item("Buff Weight")},
            {0x6AD64B73, new Item("Shard of Evil")},
            {0xC9432FED, new Item("Ageless Powder")},
            {0x4B2119DF, new Item("Drop of Joy")},
            {0x46B7459D, new Item("Dragon Orb")},
            {0xD08742EA, new Item("Holy Water")},
            {0x419AFD7A, new Item("Dingy Scale")},
            {0xD7AAFA0D, new Item("Venoct Aura")},
            {0xB0410B5F, new Item("Tattered Gauntlet")},
            {0x26710C28, new Item("Cracked Crystal")},
            {0x9C2005B1, new Item("Crystal Shard")},
            {0x0A1002C6, new Item("Clenzall")},
            {0xA9856658, new Item("Yellowed Sash")},
            {0x3FB5612F, new Item("Plain Ring")},
            {0x85E468B6, new Item("Blank Charm")},
            {0x13D46FC1, new Item("Ruby")},
            {0x82C9D051, new Item("Aquamarine")},
            {0x14F9D726, new Item("Topaz")},
            {0xF1701046, new Item("Tourmaline")},
            {0x67401731, new Item("Opal")},
            {0xDD111EA8, new Item("Emerald")},
    };
    }
}