using System;

namespace Server.Items
{
    public class DecayedCorpse : Container
    {
        private static readonly TimeSpan m_DefaultDecayTime = TimeSpan.FromMinutes(7.0);
        private DateTime m_DecayTime;
        private Timer m_DecayTimer;

        public DecayedCorpse(string name) : base(Utility.Random(0xECA, 9))
        {
            Movable = false;
            Name = name;

            BeginDecay(m_DefaultDecayTime);
        }

        public DecayedCorpse(Serial serial) : base(serial)
        {
        }

        // Do not display (x items, y stones)
        public override bool DisplaysContent => false;

        public void BeginDecay(TimeSpan delay)
        {
            m_DecayTimer?.Stop();

            m_DecayTime = Core.Now + delay;

            m_DecayTimer = new InternalTimer(this, delay);
            m_DecayTimer.Start();
        }

        public override void OnAfterDelete()
        {
            m_DecayTimer?.Stop();

            m_DecayTimer = null;
        }

        // Do not display (x items, y stones)
        public override bool CheckContentDisplay(Mobile from) => false;

        public override void AddNameProperty(IPropertyList list)
        {
            list.Add(1046414, Name); // the remains of ~1_NAME~
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, 1046414, Name); // the remains of ~1_NAME~
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_DecayTimer != null);

            if (m_DecayTimer != null)
            {
                writer.WriteDeltaTime(m_DecayTime);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        BeginDecay(m_DefaultDecayTime);

                        break;
                    }
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            BeginDecay(reader.ReadDeltaTime() - Core.Now);
                        }

                        break;
                    }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly DecayedCorpse m_Corpse;

            public InternalTimer(DecayedCorpse c, TimeSpan delay) : base(delay) => m_Corpse = c;

            protected override void OnTick()
            {
                m_Corpse.Delete();
            }
        }
    }
}
