import * as React from 'react'
import * as d3 from 'd3'
import * as ChartClient from '../ChartClient';
import * as ChartUtils from '../Templates/ChartUtils';
import { translate, scale, rotate, skewX, skewY, matrix, scaleFor, rule, ellipsis, PivotRow, groupedPivotTable } from '../Templates/ChartUtils';
import { ChartTable, ChartColumn, ChartRow } from '../ChartClient';
import ReactChartBase from './ReactChartBase';
import { XKeyTicks, YScaleTicks } from './Components/Ticks';
import Legend from './Components/Legend';
import { XAxis, YAxis } from './Components/Axis';


export default class StackedLinesChart extends ReactChartBase {

  renderChart(data: ChartTable, width: number, height: number): React.ReactElement<any> {

    var c = data.columns;
    var keyColumn = c.c0 as ChartColumn<unknown>;
    var valueColumn0 = c.c2 as ChartColumn<number>;

    var pivot = c.c1 == null ?
      ChartUtils.toPivotTable(data, c.c0!, [c.c2, c.c3, c.c4, c.c5, c.c6].filter(cn => cn != undefined) as ChartColumn<number>[]) :
      ChartUtils.groupedPivotTable(data, c.c0!, c.c1, c.c2 as ChartColumn<number>);

    var xRule = rule({
      _1: 5,
      title: 15,
      _2: 10,
      labels: parseInt(data.parameters["Horizontal Margin"]),
      _3: 5,
      ticks: 4,
      content: '*',
      _4: 10,
    }, width);
    //xRule.debugX(chart)


    var yRule = rule({
      _1: 5,
      legend: 15,
      _2: 5,
      content: '*',
      ticks: 4,
      _3: 5,
      labels0: 15,
      labels1: 15,
      _4: 10,
      title: 15,
      _5: 5,
    }, height);
    //yRule.debugY(chart);

    var keyValues: unknown[] = ChartUtils.completeValues(keyColumn, pivot.rows.map(r => r.rowValue), data.parameters['CompleteValues'], ChartUtils.insertPoint(keyColumn, valueColumn0));

    var x = d3.scaleBand()
      .domain(keyValues.map(v => keyColumn.getKey(v)))
      .range([0, xRule.size('content')]);

    var pStack = data.parameters["Stack"];

    var rowsByKey = pivot.rows.toObject(r => keyColumn.getKey(r.rowValue));

    var stack = d3.stack<unknown>()
      .offset(ChartUtils.getStackOffset(pStack)!)
      .order(ChartUtils.getStackOrder(data.parameters["Order"])!)
      .keys(pivot.columns.map(d => d.key))
      .value((r, k) => {

        var row = rowsByKey[keyColumn.getKey(r)];
        if (row == null)
          return 0;

        var v = row.values[k];
        return v && v.value || 0;
      });

    var stackedSeries = stack(keyValues);

    var max = d3.max(stackedSeries, s => d3.max(s, v => v[1]))!;
    var min = d3.min(stackedSeries, s => d3.min(s, v => v[0]))!;

    var y = d3.scaleLinear()
      .domain([min, max])
      .range([0, yRule.size('content')]);

    var color = d3.scaleOrdinal(ChartUtils.getColorScheme(data.parameters["ColorCategory"], parseInt(data.parameters["ColorCategorySteps"]))).domain(pivot.columns.map(s => s.key));

    var pInterpolate = data.parameters["Interpolate"];

    var area = d3.area<d3.SeriesPoint<unknown>>()
      .x(v => x(keyColumn.getKey(v.data))!)
      .y0(v => -y(v[0]))
      .y1(v => -y(v[1]))
      .curve(ChartUtils.getCurveByName(pInterpolate) as d3.CurveFactory);

    var columnsByKey = pivot.columns.toObject(a => a.key);

    var format = pStack == "expand" ? d3.format(".0%") :
      pStack == "zero" ? d3.format("") :
        (n: number) => d3.format("")(n) + "?";

    var rectRadious = 2;

    return (
      <svg direction="ltr" width={width} height={height}>
        <XKeyTicks xRule={xRule} yRule={yRule} keyValues={keyValues} keyColumn={keyColumn} x={x} />
        <YScaleTicks xRule={xRule} yRule={yRule} valueColumn={valueColumn0} y={y} format={format} />

        {stackedSeries.map(s => <g key={s.key} className="shape-serie"
          transform={translate(xRule.start('content') + x.bandwidth() / 2, yRule.end('content'))}>
          <path className="shape" stroke={color(s.key)} fill={color(s.key)} shapeRendering="initial" d={area(s)!}>
            <title>
              {columnsByKey[s.key].niceName!}
            </title>
          </path>
        </g>)}

        {stackedSeries.map(s => <g key={s.key} className="hover-trigger-serie"
          transform={translate(xRule.start('content') + x.bandwidth() / 2, yRule.end('content'))}>

          {s.filter(v => rowsByKey[keyColumn.getKey(v.data)] && rowsByKey[keyColumn.getKey(v.data)].values[s.key] != undefined)
            .map(v => <rect key={keyColumn.getKey(v.data)} className="point"
              x={x(keyColumn.getKey(v.data))! - rectRadious}
              y={-y(v[1])}
              width={2 * rectRadious}
              height={y(v[1]) - y(v[0])}
              fill="#fff"
              fillOpacity={.1}
              stroke="none"
              onClick={e => this.props.onDrillDown(rowsByKey[keyColumn.getKey(v.data)].values[s.key].rowClick)}
              cursor="pointer">
              <title>
                {rowsByKey[keyColumn.getKey(v.data)].values[s.key].valueTitle}
              </title>
            </rect>)}

          {x.bandwidth() > 15 && parseFloat(data.parameters["NumberOpacity"]) > 0 &&
            s.filter(v => rowsByKey[keyColumn.getKey(v.data)] && rowsByKey[keyColumn.getKey(v.data)].values[s.key] != undefined && (y(v[1]) - y(v[0])) > 10)
              .map(v => <text key={keyColumn.getKey(v.data)}
                className="number-label"
                x={x(keyColumn.getKey(v.data))!}
                y={-y(v[1]) * 0.5 - y(v[0]) * 0.5}
                fill={data.parameters["NumberColor"]}
                dominantBaseline="middle"
                opacity={data.parameters["NumberOpacity"]}
                textAnchor="middle"
                fontWeight="bold">
                {rowsByKey[keyColumn.getKey(v.data)].values[s.key].value}
                <title>
                  {rowsByKey[keyColumn.getKey(v.data)].values[s.key].valueTitle}
                </title>
              </text>)
          }
        </g>
        )}

        <Legend pivot={pivot} xRule={xRule} yRule={yRule} color={color} />

        <XAxis xRule={xRule} yRule={yRule} />
        <YAxis xRule={xRule} yRule={yRule} />
      </svg>
    );
  }
}