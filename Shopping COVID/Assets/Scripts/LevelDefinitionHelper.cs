namespace DefaultNamespace {
    public static class LevelDefinitionHelper {
        private static int currentLevelDefinitionIndex = 0;

        public static int SetNextLevelIndex() {
            return currentLevelDefinitionIndex++;
        }

        public static int GetCurrentLevelIndex() {
            return currentLevelDefinitionIndex;
        }
    }
}