import {Component} from '@angular/core';
import {ColDef} from "ag-grid-community";
import {WeatherForecastService} from "../../api/services/weather-forecast.service";
import {WeatherForecast} from "../../api/models/weather-forecast";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];
  public columnDefs: ColDef[] = [
    {headerName: 'Date', field: "date", filter: true, flex: 1},
    {headerName: 'Temp. (C)', field: "temperatureC", filter: true, flex: 1},
    {headerName: 'Temp. (F)', field: "temperatureF", filter: true, flex: 1},
    {headerName: 'Summary', field: "summary", filter: true, flex: 1}
  ];

  constructor(weatherForecastService: WeatherForecastService) {
    weatherForecastService.get$Json().subscribe(result => this.forecasts = result, error => console.error(error))
  }
}
