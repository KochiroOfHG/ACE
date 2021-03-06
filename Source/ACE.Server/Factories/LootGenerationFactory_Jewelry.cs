using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Factories;
using ACE.Server.WorldObjects;

namespace ACE.Server.Factories
{
    public static partial class LootGenerationFactory
    {
        private static WorldObject CreateJewels(int tier, bool isMagical)
        {
            uint gemType = 0;
            int workmanship = 0;
            int rank = 0;
            int difficulty = 0;
            int spellDID = 0;
            int skill_level_limit = 0;

            int gemLootMatrixIndex = tier - 1;

            if (gemLootMatrixIndex > 4) gemLootMatrixIndex = 4;
            int upperLimit = LootTables.GemsMatrix[gemLootMatrixIndex].Length - 1;

            uint gemWCID = (uint)LootTables.GemsWCIDsMatrix[gemLootMatrixIndex][ThreadSafeRandom.Next(0, upperLimit)];

            WorldObject wo = WorldObjectFactory.CreateNewWorldObject(gemWCID) as Gem;

            if (wo == null)
                return null;

            gemType = (uint)wo.MaterialType;

            workmanship = GetWorkmanship(tier);
            wo.SetProperty(PropertyInt.ItemWorkmanship, workmanship);
            int value = LootTables.gemValues[(int)gemType] + ThreadSafeRandom.Next(1, LootTables.gemValues[(int)gemType]);
            wo.SetProperty(PropertyInt.Value, value );

            gemLootMatrixIndex = tier - 1;
            if (isMagical)
            {
                wo.SetProperty(PropertyInt.ItemUseable, 8);
                wo.SetProperty(PropertyInt.UiEffects, (int)UiEffects.Magical);

                int gemSpellIndex;
                int spellChance = 0;

                spellChance = ThreadSafeRandom.Next(0, 3);
                switch (spellChance)
                {
                    case 0:
                        gemSpellIndex = LootTables.GemSpellIndexMatrix[gemLootMatrixIndex][0];
                        spellDID = LootTables.GemCreatureSpellMatrix[gemSpellIndex][ThreadSafeRandom.Next(0, LootTables.GemCreatureSpellMatrix[gemSpellIndex].Length - 1)];
                        break;
                    case 1:
                        gemSpellIndex = LootTables.GemSpellIndexMatrix[gemLootMatrixIndex][0];
                        spellDID = LootTables.GemLifeSpellMatrix[gemSpellIndex][ThreadSafeRandom.Next(0, LootTables.GemLifeSpellMatrix[gemSpellIndex].Length - 1)];
                        break;
                    case 2:
                        gemSpellIndex = LootTables.GemSpellIndexMatrix[gemLootMatrixIndex][1];
                        spellDID = LootTables.GemCreatureSpellMatrix[gemSpellIndex][ThreadSafeRandom.Next(0, LootTables.GemCreatureSpellMatrix[gemSpellIndex].Length - 1)];
                        break;
                    default:
                        gemSpellIndex = LootTables.GemSpellIndexMatrix[gemLootMatrixIndex][1];
                        spellDID = LootTables.GemLifeSpellMatrix[gemSpellIndex][ThreadSafeRandom.Next(0, LootTables.GemLifeSpellMatrix[gemSpellIndex].Length - 1)];
                        break;
                }

                int manaCost = 50 * gemSpellIndex;
                int spellcraft = 50 * gemSpellIndex;
                int maxMana = ThreadSafeRandom.Next(manaCost, manaCost + 50);

                wo.SetProperty(PropertyDataId.Spell, (uint)spellDID);
                wo.SetProperty(PropertyInt.ItemAllegianceRankLimit, rank);
                wo.SetProperty(PropertyInt.ItemDifficulty, difficulty);
                wo.SetProperty(PropertyInt.ItemManaCost, manaCost);
                wo.SetProperty(PropertyInt.ItemMaxMana, maxMana);
                wo.SetProperty(PropertyInt.ItemSkillLevelLimit, skill_level_limit);
                wo.SetProperty(PropertyInt.ItemSpellcraft, spellcraft);
            }
            else
            {
                wo.SetProperty(PropertyInt.ItemUseable, 1);

                wo.RemoveProperty(PropertyInt.ItemManaCost);
                wo.RemoveProperty(PropertyInt.ItemMaxMana);
                wo.RemoveProperty(PropertyInt.ItemCurMana);
                wo.RemoveProperty(PropertyInt.ItemSpellcraft);
                wo.RemoveProperty(PropertyInt.ItemDifficulty);
                wo.RemoveProperty(PropertyInt.ItemSkillLevelLimit);
                wo.RemoveProperty(PropertyDataId.Spell);
            }

            wo = RandomizeColor(wo);
            return wo;
        }

        private static WorldObject CreateJewelry(int tier, bool isMagical, bool lucky = false)
        {
            int[] jewelryItems = { 621, 295, 297, 294, 623, 622 };
            int jewelType = jewelryItems[ThreadSafeRandom.Next(0, jewelryItems.Length - 1)];

            //int rank = 0;
            //int skill_level_limit = 0;

            WorldObject wo = WorldObjectFactory.CreateNewWorldObject((uint)jewelType);

            if (wo == null)
                return null;

            wo.SetProperty(PropertyInt.AppraisalLongDescDecoration, 1);
            wo.SetProperty(PropertyString.LongDesc, wo.GetProperty(PropertyString.Name));
            int materialType = GetMaterialType(wo, tier);
            if (materialType > 0)
                wo.MaterialType = (MaterialType)materialType;
            int gemCount = ThreadSafeRandom.Next(1, 5);
            int gemType = ThreadSafeRandom.Next(10, 50);
            wo.SetProperty(PropertyInt.GemCount, gemCount);
            wo.SetProperty(PropertyInt.GemType, gemType);
            int workmanship = GetWorkmanship(tier);

            wo.SetProperty(PropertyInt.ItemWorkmanship, workmanship);

            wo.RemoveProperty(PropertyInt.ItemSkillLevelLimit);

            if (tier > 6)
            {
                int wield;

                wo.SetProperty(PropertyInt.WieldRequirements, (int)WieldRequirement.Level);
                wo.SetProperty(PropertyInt.WieldSkillType, (int)Skill.Axe);  // Set by examples from PCAP data

                switch (tier)
                {
                    case 7:
                        wield = 150; // In this instance, used for indicating player level, rather than skill level
                        break;
                    default:
                        wield = 180; // In this instance, used for indicating player level, rather than skill level
                        break;
                }

                wo.SetProperty(PropertyInt.WieldDifficulty, wield);
            }

            if (isMagical)
                wo = AssignMagic(wo, tier, lucky);
            else
            {
                wo.RemoveProperty(PropertyInt.ItemManaCost);
                wo.RemoveProperty(PropertyInt.ItemMaxMana);
                wo.RemoveProperty(PropertyInt.ItemCurMana);
                wo.RemoveProperty(PropertyInt.ItemSpellcraft);
                wo.RemoveProperty(PropertyInt.ItemDifficulty);
                wo.RemoveProperty(PropertyFloat.ManaRate);
            }

            wo = AssignValue(wo);

            return wo;
        }
    }
}
