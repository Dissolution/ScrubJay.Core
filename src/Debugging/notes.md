

### Optimization

#### `IL`
- Purely in IL, a local should only be declared if it is used more than 3 times

##### With Local
- declare (also requires declaring the local)
```il
ldarg.0
ldfld
stloc.0
```
  - 3x

- use
```il
ldloc.0
```
  - 1x

##### Without Local
- declare (nothing)
  - 0x
- use
```il
ldarg.0
ldfld
```
  - 2x