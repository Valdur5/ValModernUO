using Server.Items;

namespace Server.Mobiles
{
    public class RedDeath : SkeletalMount
    {
        [Constructible]
        public RedDeath() : base()
        {
            IsParagon = true;

            Hue = 0x21;
            BaseSoundID = 0x1C3;

            AI = AIType.AI_Melee;
            FightMode = FightMode.Closest;

            SetStr(319, 324);
            SetDex(241, 244);
            SetInt(242, 255);

            SetHits(1540, 1605);

            SetDamage(25, 29);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 75);
            SetDamageType(ResistanceType.Cold, 0);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 90);
            SetResistance(ResistanceType.Cold, 0);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 0);

            SetSkill(SkillName.Wrestling, 121.4, 143.7);
            SetSkill(SkillName.Tactics, 120.9, 142.2);
            SetSkill(SkillName.MagicResist, 120.1, 142.3);
            SetSkill(SkillName.Anatomy, 120.2, 144.0);

            Fame = 28000;
            Karma = -28000;

            if (Utility.RandomBool())
            {
                PackNecroScroll(Utility.RandomMinMax(5, 9));
            }
            else
            {
                PackScroll(4, 7);
            }
        }

        public RedDeath(Serial serial) : base(serial)
        {
        }

        public override string DefaultName => "Red Death";

        public override string CorpseName => "a Red Death corpse";

        public override bool GivesMLMinorArtifact => true;
        public override bool AlwaysMurderer => true;

        private static MonsterAbility[] _abilities = { MonsterAbility.ChaosBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
        }

        public override WeaponAbility GetWeaponAbility() => WeaponAbility.WhirlwindAttack;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new ResolvesBridle());
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}
