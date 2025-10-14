# Metrics
The application uses [OpenTelemetry](https://opentelemetry.io) to collect metrics and export them to [Prometheus](https://prometheus.io).

## Details

The application collects the following business-specific metrics:

### PreliminaryDataImport
- `sweather.preliminary_data_import.ace_records_created` - The number of ACE records created.
- `sweather.preliminary_data_import.ace_records_updated` - The number of ACE records updated.
- `sweather.preliminary_data_import.dscovr_records_created` - The number of DSCOVR records created.
- `sweather.preliminary_data_import.dscovr_records_updated` - The number of DSCOVR records updated.

### PreliminaryDataProcess
- `sweather.preliminary_data_process.ace_records_created` - The number of ACE records created.
- `sweather.preliminary_data_process.ace_records_updated` - The number of ACE records updated.
- `sweather.preliminary_data_process.dscovr_records_created` - The number of DSCOVR records created.
- `sweather.preliminary_data_process.dscovr_records_updated` - The number of DSCOVR records updated.

### PopularScience
- `sweather.popular_science.noaa_discussions_reloaded` - The number of NOAA discussions reloaded.
- `sweather.popular_science.noaa_discussions_translated` - The number of NOAA discussions translated.
- `sweather.popular_science.noaa_discussions_invalidated` - The number of NOAA discussions invalidated.
- `sweather.popular_science.sidc_discussions_reloaded` - The number of SIDC discussions reloaded.
- `sweather.popular_science.sidc_discussions_translated` - The number of SIDC discussions translated.
- `sweather.popular_science.sidc_discussions_invalidated` - The number of SIDC discussions invalidated.
- `sweather.popular_science.ipg_discussions_reloaded` - The number of IPG discussions reloaded.
- `sweather.popular_science.ipg_discussions_translated` - The number of IPG discussions translated.
- `sweather.popular_science.ipg_discussions_invalidated` - The number of IPG discussions invalidated.

### Visualization
- `sweather.visualization.op_noaa_reloaded` - The number of NOAA Ovation Prime reloads.
- `sweather.visualization.op_noaa_invalidated` - The number of NOAA Ovation Prime invalidations.

## Urls
- Application metrics
  - Host: http://localhost:5051/metrics
  - Front: http://localhost:5052/metrics
- Prometheus
  - Url: http://localhost:9090
  - Current targets and scraping status: http://localhost:9090/targets

## See also
- https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Prometheus.AspNetCore/README.md
- https://prometheus.io/docs/prometheus/latest/configuration/configuration/#tls_config
