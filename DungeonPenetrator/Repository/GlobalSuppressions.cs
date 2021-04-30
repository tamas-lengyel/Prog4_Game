// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1820:Test for empty strings using string length", Justification = "Not relevant.", Scope = "member", Target = "~M:Repository.HighscoreRepository.GetAll~System.Linq.IQueryable{Model.Highscore}")]
[assembly: SuppressMessage("Performance", "CA1820:Test for empty strings using string length", Justification = "Not relevant.", Scope = "member", Target = "~M:Repository.AutoSaveGameRepository.GetSaveGame~Model.GameModel")]
[assembly: SuppressMessage("Performance", "CA1820:Test for empty strings using string length", Justification = "Not relevant.", Scope = "member", Target = "~M:Repository.ManualSaveGameRepository.GetSaveGame~Model.GameModel")]
