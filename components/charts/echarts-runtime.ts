import { use } from 'echarts/core'
import { BarChart, LineChart, PieChart } from 'echarts/charts'
import {
  DatasetComponent,
  GridComponent,
  LegendComponent,
  TitleComponent,
  TooltipComponent
} from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import VChart from 'vue-echarts'

let isRegistered = false

export function getEChartComponent() {
  if (!isRegistered) {
    use([
      CanvasRenderer,
      LineChart,
      BarChart,
      PieChart,
      TitleComponent,
      TooltipComponent,
      LegendComponent,
      GridComponent,
      DatasetComponent
    ])
    isRegistered = true
  }

  return VChart
}
