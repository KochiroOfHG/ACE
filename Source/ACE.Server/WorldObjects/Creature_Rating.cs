using System;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.WorldObjects
{
    partial class Creature
    {
        // Ratings:
        // http://asheron.wikia.com/wiki/Rating

        private static float GetRatingMod(int rating)
        {
            if (rating == 0) return 1.0f;

            if (rating >= 0)
                return GetPositiveRatingMod(rating);
            else
                return GetNegativeRatingMod(-rating);
        }

        /// <summary>
        /// Returns a 1.xx rating modifier by default,
        /// or a 0.xx rating modifier if negative
        /// </summary>
        public static float GetPositiveRatingMod(int rating)
        {
            if (rating < 0) return GetNegativeRatingMod(-rating);

            // formula: (100 + rating) / 100 = 1.xx modifier
            var ratingMod = (100 + rating) / 100.0f;
            return ratingMod;
        }

        /// <summary>
        /// Returns a 0.xx rating modifier by default,
        /// or a 1.xx rating modifier if negative
        /// </summary>
        public static float GetNegativeRatingMod(int rating)
        {
            if (rating < 0) return GetPositiveRatingMod(-rating);

            // formula: 100 / (100 + rating) = 0.xx modifier
            var ratingMod = 100.0f / (100 + rating);
            return ratingMod;
        }

        // - Damage rating - increases all damage done to a target, including critical hits
        // Formula: (100 + <total damage rating>) / 100 = 1.xx modifier to damage
        public float GetDamageRating(int damageRating)
        {
            return GetPositiveRatingMod(damageRating);
        }

        // - Critical damage rating - increases critical hit damage done to a target
        // Formula: (100 + <total critical damage rating>) / 100 = 1.xx modifier to critical damage
        public float GetCriticalDamageRating(int criticalDmgRating)
        {
            return GetPositiveRatingMod(criticalDmgRating);
        }

        // - Damage resistance rating - decreases the amount of all incoming damage, including critical hits
        // Formula: 100 / (100 + <total damage resistance rating>) = 0.xxx modifier to incoming damage
        public float GetDamageResistanceRating(int damageResistanceRating)
        {
            return GetNegativeRatingMod(damageResistanceRating);
        }

        // - Critical damage resistance rating - decreases critical hit damage received
        // Formula: 100 / (100 + <total critical damage resistance rating>) = 0.xxx modifier to critical hit damage received
        public float GetCriticalDamageResistanceRating(int criticalDmgResistanceRating)
        {
            return GetNegativeRatingMod(criticalDmgResistanceRating);
        }

        // - DoT resistance rating - decreases the effectiveness of damage over time (DoT) spells cast upon the user
        // Formula: 100 / (100 + <total DoT reduction rating> = 0.xxx modifier to outgoing incoming DoT attacks
        public float GetDamageOverTimeResistanceRating(int dotResistanceRating)
        {
            return GetNegativeRatingMod(dotResistanceRating);
        }

        // - Health drain resistance rating - decreases the effect of drain spells cast upon the target
        // Formula: 100 / (100 + <total health drain resistance rating>) = 0.xxx modifier to incoming health drains
        public float GetHealthDrainResistanceRating(int healthDrainResistRating)
        {
            return GetNegativeRatingMod(healthDrainResistRating);
        }

        // - Healing boost rating - increases the amount of health received from consumables, healing kits, and life magic
        // Formula: (100 + <total healing rating>) / 100 = 1.xx modifier to healing
        public float GetHealingBoostRating(int healingBoostRating)
        {
            return GetPositiveRatingMod(healingBoostRating);
        }

        // - Aetheria Surge rating - increases the chance that Aetheria will surge
        // Formula: (100 + <aetheria surge rating>) / 100 = 1.xx modifier to Aetheria Surges
        public float GetAetheriaSurgeRating(int aetheriaSurgeRating)
        {
            return GetPositiveRatingMod(aetheriaSurgeRating);
        }

        // - Mana charge rating - increases the amount of mana released from mana stones into magical items
        // Formula: (100 + <mana charge rating>) / 100 = 1.xx modifier to mana charges
        public float GetManaChargeRating(int manaChargeRating)
        {
            return GetPositiveRatingMod(manaChargeRating);
        }

        // - Mana reduction rating - decreases the amount of mana consumed in equipped magical items
        // Formula: 100 / (100 + <mana reduction rating>) = 0.xxx modifier to magical item mana consumed
        public float GetManaReductionRating(int manaReductionRating)
        {
            return GetNegativeRatingMod(manaReductionRating);
        }

        // - Damage reduction rating - debuff applied to decrease the effective damage rating of the target
        // Formula: 100 / (100 + <total damage reduction rating>) = 0.xxx modifier to outgoing damage
        public float GetDamageReductionRating(int damageReductionRating)
        {
            return GetNegativeRatingMod(damageReductionRating);
        }

        // - Healing reduction rating - debuff applied to decrease the effective healing rate of the target
        // Formula: 100 / (100 + <total healing reduction rating>) = 0.xxx modifier to healing
        public float GetHealingReductionRating(int healingReductionRating)
        {
            return GetNegativeRatingMod(healingReductionRating);
        }

        // - Damage resistance reduction rating - debuff applied to decrease the effective damage resistance rating of the target
        // Formula: 100 / (100 + <total damage resistance reduction rating>) = 0.xxx modifier to incoming damage
        public float GetDamageResistanceReductionRating(int damageResistReduceRating)
        {
            return GetNegativeRatingMod(damageResistReduceRating);
        }

        // - Player Killer damage rating - increases all damage done to other players, including critical hits
        // Formula: (100 + <total PK damage rating> / 100 = 1.xx modifier to PK damage
        public float GetPKDamageRating(int pkDamageRating)
        {
            return GetPositiveRatingMod(pkDamageRating);
        }

        // - Player Killer damage resistance rating - decreases all incoming damage done by other players, including critical hits
        // Formula: 100 / (100 + <total PK damage resistance rating>) = 0.xxx modifier to incoming PK damage
        public float GetPKDamageResistanceRating(int pkDamageResistRating)
        {
            return GetNegativeRatingMod(pkDamageResistRating);
        }

        /// <summary>
        /// Converts a 1.xx modifier to a +x rating,
        /// or a 0.xx modifier to a -x rating.
        /// </summary>
        public static int ModToRating(float mod)
        {
            if (mod >= 1.0f)
                return (int)Math.Round(mod * 100 - 100);
            else
                return (int)Math.Round(-100 / mod + 100);
        }

        /// <summary>
        /// Returns a modifier to an implicitly negative rating (ie. damage resistance)
        /// </summary>
        public static int NegativeModToRating(float mod)
        {
            return -ModToRating(mod);
        }

        /// <summary>
        /// Combines rating modifiers additively
        /// </summary>
        /// <param name="mods">A list of rating modifiers</param>
        public static float AdditiveCombine(params float[] mods)
        {
            int totalRating = 0;

            foreach (var mod in mods)
                totalRating += ModToRating(mod);

            return GetRatingMod(totalRating);
        }

        public int GetDamageRating()
        {
            // get from base properties (monsters)?
            var damageRating = DamageRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.DamageRating);

            // weakness as negative damage rating?
            // TODO: this should be factored in as a separate weakness rating...
            var weaknessRating = EnchantmentManager.GetRating(PropertyInt.WeaknessRating);

            var augBonus = 0;
            var lumAugBonus = 0;

            if (this is Player player)
            {
                augBonus = player.AugmentationDamageBonus * 3;
                lumAugBonus = player.LumAugDamageRating;
            }

            // heritage / weapon type bonus factored in elsewhere?
            return damageRating + enchantments - weaknessRating + augBonus + lumAugBonus;
        }

        public int GetDamageResistRating()
        {
            // get from base properties (monsters)?
            var damageResistRating = DamageResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.DamageResistRating);

            // nether DoTs as negative DRR?
            // TODO: this should be factored in as a separate nether damage rating...
            var netherDotDamageRating = EnchantmentManager.GetNetherDotDamageRating();

            var augBonus = 0;
            var lumAugBonus = 0;

            if (this is Player player)
            {
                augBonus = player.AugmentationDamageReduction * 3;
                lumAugBonus = player.LumAugDamageReductionRating;
            }

            return damageResistRating + enchantments - netherDotDamageRating + augBonus + lumAugBonus;
        }

        public int GetCritRating()
        {
            // crit chance

            // get from base properties (monsters)?
            var critChanceRating = CritRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.CritRating);

            // augmentations
            var augBonus = 0;

            if (this is Player player)
                augBonus = player.AugmentationCriticalExpertise;

            return critChanceRating + enchantments + augBonus;
        }

        public int GetCritDamageRating()
        {
            // get from base properties (monsters)?
            var critDamageRating = CritDamageRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.CritDamageRating);

            // augmentations
            var augBonus = 0;
            var lumAugBonus = 0;

            if (this is Player player)
            {
                augBonus = player.AugmentationCriticalPower * 3;
                lumAugBonus = player.LumAugCritDamageRating;
            }

            return critDamageRating + enchantments + augBonus + lumAugBonus;
        }

        public int GetCritResistRating()
        {
            // crit resist chance

            // get from base properties (monsters)?
            var critResistRating = CritResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.CritResistRating);

            // no augs / lum augs?
            return critResistRating + enchantments;
        }

        public int GetCritDamageResistRating()
        {
            // get from base properties (monsters)?
            var critDamageResistRating = CritDamageResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.CritDamageResistRating);

            var lumAugBonus = 0;
            if (this is Player player)
                lumAugBonus = player.LumAugCritReductionRating;

            return critDamageResistRating + enchantments + lumAugBonus;
        }

        public int GetHealingBoostRating()
        {
            // get from base properties (monsters)?
            var healBoostRating = HealingBoostRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.HealingBoostRating);

            var lumAugBonus = 0;
            if (this is Player player)
                lumAugBonus = player.LumAugHealingRating;

            return healBoostRating + enchantments + lumAugBonus;
        }

        public int GetHealingResistRating()
        {
            // debuff?
            var healResistRating = HealingResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.HealingResistRating);

            return healResistRating + enchantments;
        }

        public int GetLifeResistRating()
        {
            // drain resistance?

            // Drain Resistances - allows one to partially resist drain health/stamina/mana and harm attacks (not including other life transfer spells).

            // get from base properties (monsters)?
            var lifeResistRating = LifeResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.LifeResistRating);

            return lifeResistRating + enchantments;
        }

        public int GetNetherResistRating()
        {
            // wiki calls this dot resistance, does this affect dirty fighting bleed attack?

            // get from base properties (monsters)?
            var netherResistRating = NetherResistRating ?? 0;

            // additive enchantments
            var enchantments = EnchantmentManager.GetRating(PropertyInt.NetherResistRating);

            return netherResistRating + enchantments;
        }

        public int GetGearMaxHealth()
        {
            // ??
            return 0;
        }

        public int GetPKDamageRating()
        {
            var pkDamageRating = PKDamageRating ?? 0;

            // additive enchantments?
            var enchantments = EnchantmentManager.GetRating(PropertyInt.PKDamageRating);

            return pkDamageRating + enchantments;
        }

        public int GetPKDamageResistRating()
        {
            var pkDamageResistRating = PKDamageResistRating ?? 0;

            // additive enchantments?
            var enchantments = EnchantmentManager.GetRating(PropertyInt.PKDamageResistRating);

            return pkDamageResistRating + enchantments;
        }

        public int GetItemManaReductionRating()
        {
            // only comes from luminance aug?
            var lumAugBonus = 0;

            if (this is Player player)
                lumAugBonus = player.LumAugItemManaUsage;

            return lumAugBonus;
        }

        public int GetManaChargeRating()
        {
            // only comes from luminance aug?
            var lumAugBonus = 0;

            if (this is Player player)
                lumAugBonus = player.LumAugItemManaGain;

            return lumAugBonus;
        }
    }
}
