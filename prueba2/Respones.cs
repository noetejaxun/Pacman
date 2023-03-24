namespace prueba2;

public class Response
    { 
        public int caminosRealizados { get; set; }
        public int caminosBuenos { get; set; }
        public int caminosOptimos { get; set; }
        public int pesoCaminoOptimo { get; set; }
        public string printedBoard { get; set; }
        public List<List<int>> board { get; set; }

    }