MedTech EMR Integration
API for syncing patient data from EMR systems and calculating loyalty points.
Setup
Requires Docker Desktop.
bashdocker-compose up --build
This starts 3 containers in one network:

MedTech API (main integration API)
Mock EMR API (simulates EMR system)
SQL Server database

Test it
Open Swagger UIs:

MedTech API: http://localhost:5001/swagger/index.html
Mock EMR API: http://localhost:5000/swagger/index.html

Use Mock EMR endpoints to simulate patient/procedure data, then check MedTech API for synced results.
How it works
EMR system sends webhook → API creates patient → calculates points for procedures
Built for MedTech integration demo.