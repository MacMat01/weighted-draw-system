using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;
using ProbabilisticEngine.Effects;
using ProbabilisticEngine.Conditions;

public static class ProbabilityEngineTest2
{
    public static void Run()
    {
        Debug.Log("=== TEST PROBABILITY ENGINE (STATEFUL, CURRENT CARD FROM DECK) ===");

        // Stato iniziale
        var state = new GameState();
        state.SetResource("Reputation", 5);
        state.SetResource("Study", 5);

        // Mazzo di 15 carte
        var deck = CreateCards();

        for (int iteration = 1; iteration <= 3; iteration++)
        {
            Debug.Log($"\n=== ITERAZIONE {iteration} ===");
            Debug.Log($"Stato iniziale: Reputation={state.GetResource("Reputation")}, Study={state.GetResource("Study")}");

            // 1) Filtra carte valide in base allo stato
            var validCards = deck
                .Where(c => c.Choice.Options.Any(o => o.AreConditionsMet(state)))
                .ToList();

            Debug.Log("Carte valide:");
            foreach (var c in validCards)
                Debug.Log($" - {c.Id}");

            // 2) Pesca la current card in base ai pesi
            var current = ChooseNextCard(validCards, state);
            Debug.Log($"Carta pescata: {current.Id}");

            // 3) Applica gli effetti della carta
            var option = current.Choice.Options[0];
            foreach (var effect in option.Effects)
                effect.Apply(state);

            Debug.Log($"Dopo effetti: Reputation={state.GetResource("Reputation")}, Study={state.GetResource("Study")}");
        }
    }

    // ------------------------------
    // CREAZIONE DEL MAZZO
    // ------------------------------

    private static List<Card> CreateCards()
{
    var cards = new List<Card>();

    // 5 carte SEMPRE disponibili
    for (int i = 0; i < 5; i++)
    {
        bool boostsStudy = (i % 2 == 0);

        string boosted = boostsStudy ? "Study" : "Reputation";
        string penalized = boostsStudy ? "Reputation" : "Study";

        cards.Add(new Card
        {
            Id = $"Card_{i}_NoConditions",
            Choice = new ProbabilityChoice
            {
                Id = $"Card_{i}",
                Options = new List<ProbabilityOption>
                {
                    new ProbabilityOption
                    {
                        Id = $"Card_{i}_Option",
                        BaseWeight = 1 + i,
                        // NESSUNA CONDIZIONE
                        Effects =
                        {
                            new ResourceEffect { Resource = boosted, Amount = +3 },
                            new ResourceEffect { Resource = penalized, Amount = -2 }
                        }
                    }
                }
            }
        });
    }

    // 10 carte con condizioni
    for (int i = 5; i < 15; i++)
    {
        bool boostsStudy = (i % 2 == 0);

        string boosted = boostsStudy ? "Study" : "Reputation";
        string penalized = boostsStudy ? "Reputation" : "Study";

        int highThreshold = 7;
        int lowThreshold = 2;

        cards.Add(new Card
        {
            Id = $"Card_{i}_Conditional",
            Choice = new ProbabilityChoice
            {
                Id = $"Card_{i}",
                Options = new List<ProbabilityOption>
                {
                    new ProbabilityOption
                    {
                        Id = $"Card_{i}_Option",
                        BaseWeight = 1 + i,

                        Conditions =
                        {
                            new ResourceCondition
                            {
                                Resource = boosted,
                                MinValue = highThreshold
                            },
                            new ResourceCondition
                            {
                                Resource = penalized,
                                MinValue = lowThreshold
                            }
                        },

                        Effects =
                        {
                            new ResourceEffect { Resource = boosted, Amount = +4 },
                            new ResourceEffect { Resource = penalized, Amount = -3 }
                        }
                    }
                }
            }
        });
    }

    return cards;
}

    // ------------------------------
    // SCELTA DELLA CARTA
    // ------------------------------

    private static Card ChooseNextCard(List<Card> validCards, GameState state)
    {
        var weights = validCards
            .Select(c => c.Choice.Options[0].ComputeWeight(state))
            .ToList();

        Debug.Log("Pesi carte valide:");
        for (int i = 0; i < validCards.Count; i++)
            Debug.Log($" - {validCards[i].Id}: weight={weights[i]}");

        int index = ProbabilisticEngine.Utils.WeightedRandom.PickIndex(weights);
        return validCards[index];
    }
}
