# IC3 Surge Lab - #4 Real-Time Analytics / Ride-Share Surge
### Plain English
It's 5pm, raining in Toronto. 100 people open Uber in Downtown, only 10 drivers there.
Normal price: $20. Surge: 2.5x = $50. That $50 gets drivers to drive downtown, supply meets demand.
If you price too low, no drivers come, 90 people wait 40 min. If too high, people walk / take TTC.
You need to count demand per area per minute, in real time. Batch nightly report is useless - surge must happen NOW.
### What It Does
1. Blazor -> Generate Normal Demand (Downtown 20 rides, Suburb 15 rides, 1.0x)
2. Blazor -> Generate Surge Demand (Downtown 120 rides, 10 drivers, rain, 5pm)
3. Kafka -> rides.requested (1000 rides)
4. Airflow -> surge_pipeline: consume -> aggregate_per_zone_1min -> calculate_surge -> build_surge_report
5. Analyst -> "Downtown: SURGE 2.5x - Demand 120, Supply 10, Ratio 12.0. Rain + rush hour. Raise price to $50, notify drivers, expect wait 4 min vs 38 min without surge."
### Key Terms
- Real-Time Analytics: Count / aggregate in minutes, not nightly. Window = 1 minute, 5 minutes.
- Surge Multiplier: demand / supply, capped. Ratio 1 = 1.0x, ratio 5 = 2.0x, ratio 12 = 2.5x.
- Supply-Demand Ratio: rides requested / drivers available in same zone + same minute.
- Sliding Window: Last 5 minutes, not all day. Old demand doesn't count.
### Run
docker compose up --build -d, Blazor 5001, Airflow 8080 trigger surge_pipeline, Analyst, Kafka UI 8085 rides.requested
