using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;

namespace Spells
{
    public class MasterYiDoubleStrike : GameScript
    {
        private Champion _owningChampion;
        private Spell _owningSpell;
        private byte _doublestrikeStacks;

        public void OnActivate(Champion owner)
        {
            _owningChampion = owner;
            ApiFunctionManager.LogInfo("MasterYi's Passive OnActivate");
            ApiEventManager.OnHitUnit.AddListener(this, owner, OnAutoAttack);
            ApiEventManager.OnDealDamage.AddListener(this, owner, champ => OnUnitDamageDealt(owner, champ));
        }
        
        private void OnUnitDamageDealt(Champion owner, AttackableUnit target)
		{
			if (target is Champion && target.IsDead && owner.GetStats().Level > 5)
			{

				float qCd = owner.GetSpellByName("AlphaStrike").getCooldown() * 0.7f;
				float wCd = owner.GetSpellByName("Meditate").getCooldown() * 0.7f;
				//float eCd = owner.GetSpellByName("MasterYiE").getCooldown() * 0.7f;
				owner.GetSpellByName("AlphaStrike").LowerCooldown(0, qCd);
				owner.GetSpellByName("Meditate").LowerCooldown(1, wCd);
				//owner.GetSpellByName("MasterYiE").LowerCooldown(2, eCd);
			}
		}

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            _owningChampion = owner;
            _owningSpell = spell;    
            
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
           
        }
        void OnAutoAttack(AttackableUnit target, bool isCrit)
        {
            
            ObjAIBase dsTarget = target as ObjAIBase;
            //1.5% bonus dmg every 4 attacks
            _doublestrikeStacks += 1;
            switch (_doublestrikeStacks)
            {
                case 1:
                    ApiFunctionManager.LogInfo("MasterYi's 1");
                    break;
                case 2:
                    ApiFunctionManager.LogInfo("MasterYi's 2");
                    break;
                case 3:
                    ApiFunctionManager.LogInfo("MasterYi's 3");
                    break;
                case 4:
                    ApiFunctionManager.LogInfo("MasterYi's 4");
                    float damage = _owningChampion.GetStats().AttackDamage.Total * 1.5f;
                    dsTarget.TakeDamage(_owningChampion, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_PASSIVE, false);
                    _doublestrikeStacks = 0;
                    break;
            }
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
        }

        public void OnUpdate(double diff)
        {
            
        }
    }
}
