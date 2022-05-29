namespace Games.TDS
{
    public class SceneSetupFactory
    {
        private ARFeatures _arFeatures;
        private SceneSetupAR.Factory _fAR;
        private SceneSetupPCTest.Factory _fPCTest;

        public SceneSetupFactory(
            ARFeatures arFeatures,
            SceneSetupAR.Factory fAR,
            SceneSetupPCTest.Factory fPCTest)
        {
            _arFeatures = arFeatures;
            _fAR = fAR;
            _fPCTest = fPCTest;
        }

        public SceneSetup Create()
        {
            if (_arFeatures.IsARSupported) {
                return _fAR.Create();
            } else {
                return _fPCTest.Create();
            }
        }
    }

}
