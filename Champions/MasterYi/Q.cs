using System;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer.Logic.API;
using System.Linq;
using LeagueSandbox.GameServer;
using System.Collections.Generic;


namespace Spells
{
    public class AlphaStrike : GameScript
    {
        private Champion _owningChampion;
        private Spell _owningSpell;


        public void OnActivate(Champion owner)
        {
            _owningChampion = owner;
        }

        public void OnDeactivate(Champion owner)
        {

        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            _owningSpell = spell;

            spell.AddProjectileTarget("AlphaStrike", target);
            _owningChampion.GetStats().Size.BaseValue = 0;
            ApiFunctionManager.CreateTimer(0.8f, () => {
                _owningChampion.GetStats().Size.BaseValue = 1;
                ApiFunctionManager.TeleportTo(owner, target.X + 80, target.Y + 80);
            });
            foreach (var enemyTarget in ApiFunctionManager.GetUnitsInRange(target, 600, true))
            {
                if (enemyTarget != target && enemyTarget != owner && target.GetDistanceTo(enemyTarget) < 100 && !ApiFunctionManager.UnitIsTurret(enemyTarget))
                {
                    ApiFunctionManager.CreateTimer(1.0f, () => {
                        ApiFunctionManager.AddParticle(owner, "MasterYi_Base_Q_tar.troy", enemyTarget.X, enemyTarget.Y);
                        spell.spellAnimation("SPELL1", owner);
                        ApiFunctionManager.AddParticleTarget(owner, "MasterYi_Base_Q_tar.troy", target);
                        ApiFunctionManager.AddParticleTarget(owner, "MasterYi_Base_Q_mis.troy", owner);
                        spell.AddProjectileTarget("AlphaStrike", enemyTarget);
                    });
                }
            }
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            foreach (var enemyTarget in ApiFunctionManager.GetUnitsInRange(target, 600, true))
            {
                if (enemyTarget != owner && !ApiFunctionManager.UnitIsTurret(enemyTarget) && ApiFunctionManager.UnitIsChampion(enemyTarget))
                {
                    float baseDamage = new[] { 25, 60, 95, 130, 165 }[spell.Level - 1] + owner.GetStats().AttackDamage.Total * 1f + owner.GetStats().AbilityPower.Total * 0.7f;
                    Boolean isCrit = new Random().Next(0, 100) < owner.GetStats().CriticalChance.Total;
                    float damage = isCrit ? baseDamage * 1.6f : baseDamage;
                    enemyTarget.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, isCrit);

                }
                else if (ApiFunctionManager.UnitIsMinion(enemyTarget))
                {
                    float baseDamage = new[] { 75, 100, 125, 150, 175 }[spell.Level - 1] + owner.GetStats().AttackDamage.Total * 1f + owner.GetStats().AbilityPower.Total * 0.7f;
                    Boolean isCrit = new Random().Next(0, 100) < owner.GetStats().CriticalChance.Total;
                    float damage = isCrit ? baseDamage * 1.6f : baseDamage;
                    enemyTarget.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, isCrit);
                }
            }
            
        }


        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            projectile.setToRemove();

        }
        public void OnUpdate(double diff)
        {
        }
    }
}
