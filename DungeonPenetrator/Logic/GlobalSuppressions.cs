// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1130:Use lambda syntax", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.InitailzeTimers")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.DropRandomCollectable")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.RandomBossMovement(System.Windows.Point)")]
[assembly: SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Count() when available", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.DropRandomCollectable")]
[assembly: SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Count() when available", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.RandomBossMovement(System.Windows.Point)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.#ctor(Model.IGameModel)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.CollectPowerup(Model.Passive.Powerups)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.DamageActiveGameObject(Model.ActiveGameObjects,System.Int32)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.MoveFlyingEnemy(Model.Active.FlyingEnemy)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.MoveProjectile(Model.Projectile@)")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.MoveRegularEnemy(Model.ActiveGameObjects)")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.ManageIntersectsForPlayer")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.GameLogic.ManageProjectileIntersects")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateBasicEnemies")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateCollectables")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateEmptySpacesForRow~System.Int32[]")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateMap")]
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateProps")]
[assembly: SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Not relevant.", Scope = "member", Target = "~M:Logic.LoadingLogic.GenerateMap")]
