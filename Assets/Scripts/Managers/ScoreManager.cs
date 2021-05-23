using System;
using Events;
using Types;

namespace Managers
{
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        public ScoreUpdatedEvent onScoreUpdated;
        public int Score { get; private set; }
        private const int AwacsScore = 100;
        private const int HawkeyeScore = 50;
        private const int SeahawkScore = 10;
        private const int GunUpgradeCost = 500;
        private const int BulletUpgradeCost = 100;
        private const int FireRateUpgradeCost = 250;

        public int GunLevel { get; private set; }
        public int BulletLevel { get; private set; }
        public int FireRateLevel { get; private set; }

        private void Start()
        {
            Score = 0;
            onScoreUpdated.AddListener(UIManager.Instance.HandleScoreUpdated);
            GunLevel = 1;
            BulletLevel = 1;
            FireRateLevel = 1;
        }
        
        public void EnemyDownedHandler(string enemyTag)
        {
            switch (enemyTag)
            {
                case "EnemyAwacs":
                    Score += AwacsScore;
                    break;
                case "EnemyHawkeye":
                    Score += HawkeyeScore;
                    break;
                case "EnemySeahawk":
                    Score += SeahawkScore;
                    break;
                default:
                    return;
            }
            onScoreUpdated.Invoke(Score);
        }

        public int GetUpgradeGunsCost() => GunUpgradeCost * GunLevel;
        public int GetUpgradeBulletsCost() => BulletUpgradeCost * BulletLevel;
        public int GetUpgradeFireRateCost() => FireRateUpgradeCost * FireRateLevel;

        public void UpgradeGuns()
        {
            if (GunLevel == 3) return;
            if (Score < GetUpgradeGunsCost()) return;
            Score -= GunUpgradeCost * GunLevel;
            onScoreUpdated.Invoke(Score);
            ++GunLevel;
            FighterManager.Instance.UpgradeGuns(GunLevel);
        }

        private void UpgradeBullets()
        {
            if (Score < GetUpgradeBulletsCost()) return;
            Score -= BulletUpgradeCost * BulletLevel;
            onScoreUpdated.Invoke(Score);
            ++BulletLevel;
            FighterManager.Instance.UpgradeBulletDamage(BulletLevel);
        }

        private void UpgradeFireRate()
        {
            if (Score < GetUpgradeFireRateCost()) return;
            Score -= FireRateUpgradeCost * FireRateLevel;
            onScoreUpdated.Invoke(Score);
            ++FireRateLevel;
            FighterManager.Instance.UpgradeFireRate(FireRateLevel);
        }
    }
}