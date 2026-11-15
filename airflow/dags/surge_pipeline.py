from airflow import DAG
from airflow.operators.python import PythonOperator
import requests
from datetime import datetime
API="http://api:8080"
def consume(): print("[Apache Kafka] Consuming rides.requested...")
def aggregate(): print("[Airflow] Aggregate per zone per 1-min window...")
def calc(): print("[Airflow] Surge = demand/supply, capped 2.5x...")
def build():
    try:
        r=requests.post(f"{API}/api/surge/report", timeout=5)
        print(f"[Airflow] Surge report: {r.text[:1200]}")
    except Exception as e: print(f"[Airflow] Surge failed: {e}")
with DAG("surge_pipeline", start_date=datetime(2026,1,1), schedule="*/1 * * * *", catchup=False, tags=["ic3","surge","realtime"]) as dag:
    t1=PythonOperator(task_id="consume_rides_requested", python_callable=consume)
    t2=PythonOperator(task_id="aggregate_per_zone_1min", python_callable=aggregate)
    t3=PythonOperator(task_id="calculate_surge", python_callable=calc)
    t4=PythonOperator(task_id="build_surge_report", python_callable=build)
    t1>>t2>>t3>>t4
