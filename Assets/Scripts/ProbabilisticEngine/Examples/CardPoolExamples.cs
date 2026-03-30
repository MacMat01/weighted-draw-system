using UnityEngine;
using System.Collections.Generic;
using ProbabilisticEngine.Core;
using ProbabilisticEngine.Interfaces;

namespace ProbabilisticEngine.Examples
{
    // Stato semplice per l'esempio
    public class SimpleGameState : IGameState
    {
        public int Health { get; set; } = 100;
        public int Mana { get; set; } = 50;
        public bool HasSword { get; set; } = false;
        public int Turn { get; set; } = 1;
    }

    // Opzione semplice
    public class SimpleOption : IProbabilityOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float BaseWeight { get; set; } = 1.0f;

        public float ComputeWeight(IGameState state)
        {
            // Valutazioni fittizzie hardcodate
            return Id switch
            {
                "attack" => 1.5f,
                "defend" => 0.8f,
                "heal" => 0.6f,
                "magic" => 1.2f,
                _ => 1.0f
            };
        }
    }

    // Condizione semplice
    public class SimpleCondition<TState> : ICondition<TState>
        where TState : SimpleGameState
    {
        public string Type { get; set; }

        public bool Evaluate(TState state)
        {
            // Valutazioni fittizzie hardcodate
            return Type switch
            {
                "has_sword" => true,
                "low_health" => false,
                "has_mana" => true,
                "first_turn" => state.Turn == 1,
                _ => true
            };
        }
    }

    // Pool di carte di esempio
    public static class CardPoolExample
    {
        public static void RunCardPoolTest()
        {
            // Stato di gioco
            var gameState = new SimpleGameState
            {
                Health = 100,
                Mana = 50,
                HasSword = false,
                Turn = 1
            };

            // Pool di carte (ProbabilityItem)
            var cardPool = new List<ProbabilityItem<SimpleGameState, SimpleOption>>
            {
                // Carta 1: Attacco base
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "basic_attack",
                    BaseWeight = 1.0f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "has_sword" }
                    },
                    Options = new List<SimpleOption>
                    {
                        new SimpleOption { Id = "attack", Name = "Basic Attack" }
                    }
                },

                // Carta 2: Difesa
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "defense",
                    BaseWeight = 0.8f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "low_health" }
                    },
                    Options = new List<SimpleOption>
                    {
                        new SimpleOption { Id = "defend", Name = "Defend" }
                    }
                },

                // Carta 3: Guarigione
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "healing",
                    BaseWeight = 0.6f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "has_mana" }
                    },
                    Options = new List<SimpleOption>
                    {
                        new SimpleOption { Id = "heal", Name = "Heal" }
                    }
                },

                // Carta 4: Magia
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "magic_spell",
                    BaseWeight = 1.2f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "first_turn" }
                    },
                    Options = new List<SimpleOption>
                    {
                        new SimpleOption { Id = "magic", Name = "Magic Spell" }
                    }
                },

                // Carta 5: Attacco speciale
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "special_attack",
                    BaseWeight = 0.4f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "has_sword" },
                        new SimpleCondition<SimpleGameState> { Type = "has_mana" }
                    },
                    Options = new List<SimpleOption>
                    {
                        new SimpleOption { Id = "attack", Name = "Special Attack" },
                        new SimpleOption { Id = "magic", Name = "Magic Attack" }
                    }
                }
            };

            // Crea l'engine con il pool di carte
            var engine = new ProbabilityEngine<SimpleGameState, SimpleOption>(cardPool);

            // Test: pesca 5 carte randomiche
            Debug.Log("=== PESCA CARTE RANDOMICHE ===\n");

            for (int i = 0; i < 5; i++)
            {
                // Filtra carte valide per lo stato corrente
                var validCards = engine.GetValidChoices(gameState);
                Debug.Log($"Turno {i + 1}: {validCards.Count} carte valide disponibili");

                if (validCards.Count > 0)
                {
                    // Pesca una carta randomica
                    var drawnCard = engine.EvaluateRandom(gameState);
                    Debug.Log($"Carta pescata: {drawnCard.Id}");

                    // Simula l'uso della carta (cambia lo stato)
                    gameState.Turn++;
                    if (drawnCard.Id == "basic_attack") gameState.HasSword = true;
                    if (drawnCard.Id == "healing") gameState.Mana -= 10;
                }
                else
                {
                    Debug.Log("Nessuna carta disponibile!");
                    break;
                }

                Debug.Log("");
            }
        }

        // Pool di carte più grande per test più completo
        public static List<ProbabilityItem<SimpleGameState, SimpleOption>> CreateLargeCardPool()
        {
            return new List<ProbabilityItem<SimpleGameState, SimpleOption>>
            {
                // Carte comuni
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "strike",
                    BaseWeight = 1.0f,
                    Options = new List<SimpleOption> { new SimpleOption { Id = "attack", Name = "Strike" } }
                },

                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "block",
                    BaseWeight = 0.9f,
                    Options = new List<SimpleOption> { new SimpleOption { Id = "defend", Name = "Block" } }
                },

                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "rest",
                    BaseWeight = 0.7f,
                    Options = new List<SimpleOption> { new SimpleOption { Id = "heal", Name = "Rest" } }
                },

                // Carte rare
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "fireball",
                    BaseWeight = 0.3f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "has_mana" }
                    },
                    Options = new List<SimpleOption> { new SimpleOption { Id = "magic", Name = "Fireball" } }
                },

                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "berserk",
                    BaseWeight = 0.2f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "low_health" }
                    },
                    Options = new List<SimpleOption> { new SimpleOption { Id = "attack", Name = "Berserk" } }
                },

                // Carte leggendarie
                new ProbabilityItem<SimpleGameState, SimpleOption>
                {
                    Id = "ultimate_spell",
                    BaseWeight = 0.1f,
                    Conditions = new List<ICondition<SimpleGameState>>
                    {
                        new SimpleCondition<SimpleGameState> { Type = "first_turn" },
                        new SimpleCondition<SimpleGameState> { Type = "has_mana" }
                    },
                    Options = new List<SimpleOption> { new SimpleOption { Id = "magic", Name = "Ultimate Spell" } }
                }
            };
        }

        // Test con pool grande
        public static void RunLargePoolTest()
        {
            var gameState = new SimpleGameState { Health = 100, Mana = 50, Turn = 1 };
            var largePool = CreateLargeCardPool();
            var engine = new ProbabilityEngine<SimpleGameState, SimpleOption>(largePool);

            Debug.Log("=== TEST POOL GRANDE ===\n");

            for (int i = 0; i < 10; i++)
            {
                var validCards = engine.GetValidChoices(gameState);
                Debug.Log($"Pesca {i + 1}: {validCards.Count} carte valide");

                if (validCards.Count > 0)
                {
                    var drawnCard = engine.EvaluateRandom(gameState);
                    Debug.Log($"→ {drawnCard.Id}");
                }

                gameState.Turn++;
            }
        }
    }
}
