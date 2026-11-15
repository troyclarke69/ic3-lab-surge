namespace SurgeLab.Api;
public record RideRequest(string Zone,int Drivers,double BasePrice,string Weather,string TimeOfDay);
public record SurgeReport(List<RideRequest> Rides,int DowntownDemand,int DowntownSupply,double Ratio,double SurgeMultiplier,double SurgePrice,DateTime GeneratedAt);
public interface IRideStore{Task<object> GenerateAsync(string mode,IKafkaProducer kafka); List<RideRequest> All();}
public class InMemoryRideStore:IRideStore{private readonly List<RideRequest> _r=new(); public List<RideRequest> All()=>_r; public async Task<object> GenerateAsync(string mode,IKafkaProducer k){int start=_r.Count; var rnd=new Random(7); if(mode=="normal"){for(int i=0;i<200;i++){var rec=new RideRequest(i%2==0?"Downtown":"Suburb",15,20,"Clear","2pm"); _r.Add(rec); await k.PublishAsync("rides.requested",rec);}} else {for(int i=0;i<800;i++){var rec=new RideRequest("Downtown",10,20,"Rain","5pm"); _r.Add(rec); await k.PublishAsync("rides.requested",rec);}} return new{added=_r.Count-start,total=_r.Count,mode};}}
public interface IKafkaProducer{Task PublishAsync(string topic,object payload);}
public interface ISurgeService{Task<SurgeReport> ReportAsync();}
public interface IAnalystAgent{Task<object> AnalyzeAsync(SurgeReport r);}
