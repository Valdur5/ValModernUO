namespace Server.Mobiles
{
    public class HellSteed : BaseMount
    {
        public override string DefaultName => "a frenzied ostard";

        [Constructible]
        public HellSteed() : base(793, 0x3EBB, AIType.AI_Animal, FightMode.Aggressor)
        {
            SetStats(this);
        }

        public HellSteed(Serial serial) : base(serial)
        {
        }

        public override string CorpseName => "a hellsteed corpse";
        public override Poison PoisonImmune => Poison.Lethal;

        private static MonsterAbility[] _abilities = { MonsterAbility.ChaosBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public static void SetStats(BaseCreature steed)
        {
            steed.SetStr(201, 210);
            steed.SetDex(101, 110);
            steed.SetInt(101, 115);

            steed.SetHits(201, 220);

            steed.SetDamage(20, 24);

            steed.SetDamageType(ResistanceType.Physical, 25);
            steed.SetDamageType(ResistanceType.Fire, 75);

            steed.SetResistance(ResistanceType.Physical, 60, 70);
            steed.SetResistance(ResistanceType.Fire, 90);
            steed.SetResistance(ResistanceType.Poison, 100);

            steed.SetSkill(SkillName.MagicResist, 90.1, 110.0);
            steed.SetSkill(SkillName.Tactics, 50.0);
            steed.SetSkill(SkillName.Wrestling, 90.1, 110.0);

            steed.Fame = 0;
            steed.Karma = 0;
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
