<template>
  <section id="matrix" class="matrix-section">
    <div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
      <header class="matrix-heading"><span>INTERACTIVE EXPERIMENT / 002</span><h2>矩阵，改变空间的方式。</h2><p>拖动参数，看二维图形被拉伸、旋转，或坍缩成一条线、一个点。</p></header>
      <div class="matrix-layout">
        <div class="matrix-controls">
          <h3>变换矩阵 A</h3>
          <div class="matrix-equation"><span>y =</span><div class="matrix-values"><b v-for="(value, i) in values" :key="i">{{ value.toFixed(2) }}</b></div><span>x</span></div>
          <label v-for="(name, i) in names" :key="name" class="matrix-slider"><span>{{ name }} <output>{{ values[i].toFixed(2) }}</output></span><input v-model.number="values[i]" type="range" min="-2" max="2" step="0.05" :aria-label="`矩阵参数 ${name}`" /></label>
          <div class="matrix-presets"><AppButton v-for="preset in presets" :key="preset.name" size="sm" variant="secondary" @click="values = [...preset.value]">{{ preset.name }}</AppButton></div>
          <p class="matrix-hint">试一试：选择「单位矩阵」，把 d 从 1 拖到 0，再继续拖到 −1。</p>
        </div>
        <div class="matrix-visual">
          <div class="matrix-legend"><span>虚线：原图</span><span class="matrix-result-key">实线：A 作用后的图形</span></div>
          <svg viewBox="0 0 600 500" role="img" :aria-label="`矩阵秩为 ${rank}，${description}`">
            <defs><marker id="matrix-arrow-first" viewBox="0 0 10 10" refX="8" refY="5" markerWidth="5" markerHeight="5" orient="auto-start-reverse"><path d="M 0 0 L 10 5 L 0 10 z" class="matrix-first-fill" /></marker><marker id="matrix-arrow-second" viewBox="0 0 10 10" refX="8" refY="5" markerWidth="5" markerHeight="5" orient="auto-start-reverse"><path d="M 0 0 L 10 5 L 0 10 z" class="matrix-second-fill" /></marker></defs>
            <g class="matrix-grid"><path v-for="n in 11" :key="`x${n}`" :d="`M ${50*n} 0 V 500`" /><path v-for="n in 9" :key="`y${n}`" :d="`M 0 ${50*n} H 600`" /></g>
            <path class="matrix-axis" d="M 0 250 H 600 M 300 0 V 500" />
            <g class="matrix-ticks"><text v-for="n in [-4,-2,2,4]" :key="n" :x="300+n*50" y="268">{{ n }}</text><text x="310" y="240">0</text><text x="584" y="240">x</text><text x="310" y="16">y</text><text x="310" y="154">2</text><text x="310" y="354">−2</text></g>
            <polygon :points="points(square)" class="matrix-original" /><path :d="circlePath(false)" class="matrix-original" />
            <polygon :points="points(square.map(transform))" class="matrix-shape" /><path :d="circlePath(true)" class="matrix-circle" />
            <g><circle v-for="(p,i) in square.map(transform)" :key="i" :cx="sx(p[0])" :cy="sy(p[1])" r="4" class="matrix-first-fill" /></g>
            <line v-if="values[0] !== 0 || values[2] !== 0" x1="300" y1="250" :x2="sx(values[0])" :y2="sy(values[2])" class="matrix-vector-first" marker-end="url(#matrix-arrow-first)" />
            <line v-if="values[1] !== 0 || values[3] !== 0" x1="300" y1="250" :x2="sx(values[1])" :y2="sy(values[3])" class="matrix-vector-second" marker-end="url(#matrix-arrow-second)" />
            <circle v-if="rank === 0" cx="300" cy="250" r="7" class="matrix-first-fill" />
          </svg>
          <div class="matrix-vector-labels"><span>青色 A·e₁ = ({{ values[0].toFixed(2) }}, {{ values[2].toFixed(2) }})</span><span>紫色 A·e₂ = ({{ values[1].toFixed(2) }}, {{ values[3].toFixed(2) }})</span></div>
        </div>
      </div>
      <div class="matrix-explanation" aria-live="polite"><div><small>矩阵的秩</small><strong>{{ rank }} <span>{{ rank === 2 ? '满秩' : '降秩' }}</span></strong></div><div><small>det(A) = ad − bc</small><strong>{{ determinant.toFixed(4) }}</strong></div><div><small>面积缩放倍数 |det(A)|</small><strong>{{ Math.abs(determinant).toFixed(4) }}×</strong></div><p>{{ description }}<br /><span>{{ orientation }}</span></p></div>
      <p class="matrix-hint">虚线正方形为 [0, 1] × [0, 1]，虚线圆为单位圆。箭头是两个基向量的像；秩描述整个二维空间的像的维数，圆周本身是曲线。</p>
    </div>
  </section>
</template>
<script setup lang="ts">
import { computed, ref } from 'vue'
import AppButton from '~/components/ui/AppButton.vue'
import '~/assets/css/matrix-lab.css'
type Point = [number, number]
const values = ref<number[]>([1, 0, 0, 1])
const names = ['a · 第一行第一列', 'b · 第一行第二列', 'c · 第二行第一列', 'd · 第二行第二列']
const presets = [
  {name:'单位矩阵',value:[1,0,0,1]}, {name:'拉伸 / 满秩',value:[1.5,.5,0,1]},
  {name:'旋转 90°',value:[0,-1,1,0]}, {name:'压成直线',value:[1,1,.5,.5]},
  {name:'压成点',value:[0,0,0,0]}, {name:'镜像翻转',value:[1,0,0,-1]}
]
// Sliders use multiples of 0.05. Integer arithmetic avoids rounding a singular matrix to full rank.
const determinant = computed(() => (Math.round(values.value[0]*20)*Math.round(values.value[3]*20)-Math.round(values.value[1]*20)*Math.round(values.value[2]*20))/400)
const rank = computed(() => values.value.every(v => v === 0) ? 0 : determinant.value === 0 ? 1 : 2)
const description = computed(() => rank.value === 2 ? '仍然是二维：正方形变成有面积的平行四边形，单位圆变成椭圆。' : rank.value === 1 ? '只剩一个方向：两个基向量的像共线，图形被压成线段，面积为 0。' : '所有方向都消失：每个点都映到原点，图形只剩一个点。')
const orientation = computed(() => rank.value < 2 ? '信息丢失，无法唯一还原原来的点。' : determinant.value < 0 ? '行列式为负：方向翻转，但仍可逆。' : '行列式为正：方向保持，变换可逆。')
const square: Point[] = [[0,0],[1,0],[1,1],[0,1]]
const sx = (x:number) => 300+x*50, sy = (y:number) => 250-y*50
const transform = ([x,y]: Point): Point => [values.value[0]*x+values.value[1]*y,values.value[2]*x+values.value[3]*y]
const points = (ps:Point[]) => ps.map(p => `${sx(p[0])},${sy(p[1])}`).join(' ')
function circlePath(transformed:boolean) { return Array.from({length:129},(_,i) => { const angle=i*Math.PI*2/128; const p:Point=[Math.cos(angle),Math.sin(angle)]; const q=transformed ? transform(p) : p; return `${i ? 'L' : 'M'} ${sx(q[0])} ${sy(q[1])}` }).join(' ') + ' Z' }
</script>
