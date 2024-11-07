using System.Collections.Generic;
using System.Linq;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace OC.Communication.TwinCAT
{
    public static class TcAdsExtension
    {
        public enum TcClientState
        {
            Stop,
            Run
        }
        
        public static bool IsRunning(this IAdsConnection client)
        {
            try
            {
                if (!client.IsConnected)
                {
                    return false;
                }
                return client.TryReadState(out var stateInfo) == AdsErrorCode.NoError && stateInfo.AdsState == AdsState.Run;
            }
            catch
            {
                return false;
            }
        }

        public static TcClientState GetTcClientState(this IAdsConnection client)
        {
            try
            {
                if (client.IsConnected && client.TryReadState(out var stateInfo) == AdsErrorCode.NoError && stateInfo.AdsState == AdsState.Run)
                {
                    return TcClientState.Run;
                }
                
                return TcClientState.Stop;
            }
            catch
            {
                return TcClientState.Stop;
            }
        }
        
        /// <summary>
        /// Get all base type symbols in main program
        /// </summary>
        public static List<ISymbol> PlcSymbols(this IAdsSymbolLoader symbolLoader)
        {
            var symbols = new List<ISymbol>();
            
            foreach (var symbol in symbolLoader.Symbols.Where(x => x.InstancePath.ToLower().StartsWith("main.")))
            {
                symbol.AddToCollection(symbols);
            }

            return symbols;
        }
        
        /// <summary>
        /// Symbols filtered by attribute
        /// </summary>
        public static IEnumerable<ISymbol> FilteredSymbols(this IEnumerable<ISymbol> symbols, string attribute)
        {
            return symbols.Where(symbol => symbol.Attributes.Any(x => x.Name.ToLower() == attribute)).ToList();
        }
        
        private static void AddToCollection(this ISymbol symbol, ICollection<ISymbol> symbols)
        {
            if (symbol.TypeName.IsTcArrayType()) return;
            
            if (symbol.TypeName.IsTcBaseType())
            {
                symbols.Add(symbol);
                return;
            }
            
            //Recursive call 
            foreach (var subSymbol in symbol.SubSymbols)
            {
                subSymbol.AddToCollection(symbols);
            }
        }
    }
}