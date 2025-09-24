using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Creature;

namespace CSTextSpireTests {
    public class PlayerTests {
        private static Deck StarterDeck() =>
        new Deck(new[] { new Card("Strike", 6), new Card("Defend", 0) });

        [Fact]
        public void EmptyNameRejectTest() {
            Assert.Throws<ArgumentException>(() => new Player("", 30, StarterDeck()));
        }
        [Fact]
        public void NonPositiveHpRejectTest() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Player("Hero", 0, StarterDeck()));
        }

        [Fact]
        public void NullDeckRejectTest() {
            Assert.Throws<ArgumentNullException>(() => new Player("Hero", 30, null!));
        }

        [Fact]
        public void TakeDamageNegativeValueDoesNotReduceHp() {
            var p = new Player("Hero", 30, StarterDeck());
            p.TakeDamage(-5);
            Assert.Equal(p.MaxHp, p.Hp);
        }

        [Fact]
        public void HealNegativeValueDoesNotIncreaseHp() {
            var p = new Player("Hero", 30, StarterDeck());
            p.Heal(-10);
            Assert.Equal(p.MaxHp, p.Hp);
        }

        [Fact]
        public void ExcessiveDamageKillsPlayer() {
            var p = new Player("Hero", 30, StarterDeck());
            p.TakeDamage(999);
            Assert.Equal(0, p.Hp);
            Assert.True(p.IsDead);
        }

        [Fact]
        public void ExcessiveHealCapsAtMaxHp() {
            var p = new Player("Hero", 30, StarterDeck());
            p.TakeDamage(10);
            p.Heal(999);
            Assert.Equal(p.MaxHp, p.Hp);
        }
    }
}
