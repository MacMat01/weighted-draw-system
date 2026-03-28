using System;
using System.Collections.Generic;
using System.Linq;
using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;
using ProbabilisticEngine.Effects;
using ProbabilisticEngine.Conditions;
using UnityEngine;

public static class ProbabilityEngineTest
{
    public static void Run()
    {
        Debug.Log("=== TEST PROBABILITY ENGINE ===");

        // Stato iniziale
        var state = new GameState();
        state.SetResource("Reputation", 5);

        // 1) Carta corrente
        var currentCard = CreateCurrentCard();

        // 2) 9 carte candidate
        var nextCards = CreateCandidateCards();

        // 3) Simula la scelta del giocatore
        var result = currentCard.Choice.Evaluate(state);

        Debug.Log($"Scelta effettuata: {result.Option.Id}");

        // 4) Applica gli effetti della scelta
        foreach (var effect in result.Option.Effects)
            effect.Apply(state);

        Debug.Log($"Reputation dopo effetti: {state.GetResource("Reputation")}");

        // 5) Filtra le carte valide
        var validCards = nextCards
            .Where(c => c.Choice.Options.Any(o => o.AreConditionsMet(state)))
            .ToList();

        Debug.Log("\nCarte valide dopo condizioni:");
        foreach (var c in validCards)
            Debug.Log($" - {c.Id}");

        // 6) Scegli la prossima carta tra quelle valide
        var next = ChooseNextCard(validCards, state);

        Debug.Log($"\nProssima carta selezionata: {next.Id}");
    }

    // ------------------------------
    // CREAZIONE CARTE
    // ------------------------------

    private static Card CreateCurrentCard()
    {
        return new Card
        {
            Id = "CurrentCard",
            Choice = new ProbabilityChoice
            {
                Id = "CurrentCard",
                Options = new List<ProbabilityOption>
                {
                    new ProbabilityOption
                    {
                        Id = "Left",
                        BaseWeight = 1,
                        Effects =
                        {
                            new ResourceEffect { Resource = "Reputation", Amount = -3 }
                        }
                    },
                    new ProbabilityOption
                    {
                        Id = "Right",
                        BaseWeight = 1,
                        Effects =
                        {
                            new ResourceEffect { Resource = "Reputation", Amount = +4 }
                        }
                    }
                }
            }
        };
    }

    private static List<Card> CreateCandidateCards()
    {
        var cards = new List<Card>();

        for (int i = 0; i < 9; i++)
        {
            cards.Add(new Card
            {
                Id = $"Card_{i}",
                Choice = new ProbabilityChoice
                {
                    Id = $"Card_{i}",
                    Options = new List<ProbabilityOption>
                    {
                        new ProbabilityOption
                        {
                            Id = $"Card_{i}_Option",
                            BaseWeight = 1 + i, // pesi diversi per testare la randomizzazione
                            Conditions =
                            {
                                new ResourceCondition
                                {
                                    Resource = "Reputation",
                                    MinValue = i - 2 // alcune carte verranno escluse
                                }
                            }
                        }
                    }
                }
            });
        }

        return cards;
    }

    // ------------------------------
    // SCELTA DELLA PROSSIMA CARTA
    // ------------------------------

    private static Card ChooseNextCard(List<Card> validCards, GameState state)
    {
        var weights = validCards
            .Select(c => c.Choice.Options[0].ComputeWeight(state))
            .ToList();

        int index = ProbabilisticEngine.Utils.WeightedRandom.PickIndex(weights);
        return validCards[index];
    }
}

// ------------------------------
// MODELLO CARTA
// ------------------------------

public class Card
{
    public string Id;
    public ProbabilityChoice Choice;
}