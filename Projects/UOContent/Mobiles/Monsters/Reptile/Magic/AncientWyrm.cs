namespace Server.Mobiles
{
    public class AncientWyrm : BaseCreature
    {
        [Constructible]
        public AncientWyrm() : base(AIType.AI_Mage)
        {
            Body = 46;
            BaseSoundID = 362;

            SetStr(1096, 1185);
            SetDex(86, 175);
            SetInt(686, 775);

            SetHits(658, 711);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 80.1, 100.0);
            SetSkill(SkillName.Meditation, 52.5, 75.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 70;
        }

        public AncientWyrm(Serial serial) : base(serial)
        {
        }

        public override string CorpseName => "a dragon corpse";
        public override string DefaultName => "an ancient wyrm";

        public override bool ReacquireOnMovement => true;
        public override bool AutoDispel => true;
        public override HideType HideType => HideType.Barbed;
        public override int Hides => 40;
        public override int Meat => 19;
        public override int Scales => 12;
        public override ScaleType ScaleType => (ScaleType)Utility.Random(4);
        public override Poison PoisonImmune => Poison.Regular;
        public override Poison HitPoison => Utility.RandomBool() ? Poison.Lesser : Poison.Regular;
        public override int TreasureMapLevel => 5;
        public override bool CanFly => true;

        private static MonsterAbility[] _abilities = { MonsterAbility.FireBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override int GetIdleSound() => 0x2D3;

        public override int GetHurtSound() => 0x2D1;

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}
